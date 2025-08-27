using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace ImageLoop3D
{
    internal class ImageLoop3DEffectProcessor : IVideoEffectProcessor
    {
        DisposeCollector disposer = new();

        IGraphicsDevicesAndContext devices;
        
        ImageLoop3DEffect item;

        ID2D1Image? input;
        ID2D1CommandList? commandList;

        bool isFirst = true;
        bool isInputChanged;
        int count;
        float x, y, z;
        Vector3 rotation;
        Matrix4x4 camera;

        public ID2D1Image Output => commandList ?? throw new NullReferenceException(nameof(commandList) + " is null");

        public ImageLoop3DEffectProcessor(IGraphicsDevicesAndContext devices, ImageLoop3DEffect item)
        {
            this.devices = devices;
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var count = (int)item.Count.GetValue(frame, length, fps);
            var x = (float)item.X.GetValue(frame, length, fps);
            var y = (float)item.Y.GetValue(frame, length, fps);
            var z = (float)item.Z.GetValue(frame, length, fps);
          
            var rotation = effectDescription.DrawDescription.Rotation;
            var camera = effectDescription.DrawDescription.Camera;

            if (isFirst || isInputChanged || this.count != count || this.x != x || this.y != y || this.z != z || this.rotation != rotation || this.camera != camera)
            {
                if (commandList is not null)
                {
                    disposer.RemoveAndDispose(ref commandList);
                }

                var dc = devices.DeviceContext;
                commandList = dc.CreateCommandList();
                disposer.Collect(commandList);
                dc.Target = commandList;
                dc.BeginDraw();
                dc.Clear(null);

                void draw(int i)
                {
                    using var renderEffect = new Transform3D(dc);

                    renderEffect.SetInput(0, input, true);

                    renderEffect.TransformMatrix = Matrix4x4.CreateRotationZ((float)Math.PI * rotation.Z / 180) *
                                                   Matrix4x4.CreateRotationY((float)Math.PI * -rotation.Y / 180) *
                                                   Matrix4x4.CreateRotationX((float)Math.PI * -rotation.X / 180) *
                                                   Matrix4x4.CreateTranslation(new Vector3(x, y, z) * i) *
                                                   effectDescription.DrawDescription.Camera *
                                                   new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, -0.001f, 0f, 0f, 0f, 1f);

                    dc.DrawImage(renderEffect.Output);
                }

                if (z > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        draw(i);
                    }
                }
                else
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        draw(i);
                    }
                }
                
                dc.EndDraw();
                dc.Target = null;
                commandList.Close();
                
                isFirst = false;
                isInputChanged = false;
                this.count = count;
                this.x = x;
                this.y = y;
                this.z = z;
                this.rotation = rotation;
                this.camera = camera;
            }

            return effectDescription.DrawDescription with
            {
                Rotation = default,
                Camera = Matrix4x4.Identity
            };
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
            isInputChanged = true;
        }

        public void ClearInput()
        {
        }

        public void Dispose()
        {
            disposer.Dispose();
        }
    }
}
