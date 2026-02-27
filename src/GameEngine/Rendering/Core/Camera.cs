using System.Numerics;
using GameEngine.Core.Application;
using GameEngine.Core.Math;

namespace GameEngine.Rendering.Core
{
    public class Camera
    {
        public Vector3 Position;

        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; }

        private Vector2 projectionSize = new Vector2(6, 3);
        private float Zoom = 1f;
        public static Camera Main = null;
        public bool Orthographic = true;
        Matrix4x4 projectionMatrix = new Matrix4x4();
        Matrix4x4 viewMatrix = new Matrix4x4();
        Matrix4x4 InverseProjMatrix = new Matrix4x4();
        Matrix4x4 InverseViewMatrix = new Matrix4x4();

        public Camera(Vector3 position)
        {
            this.Position = position;
            Main = this;
        }
        public void ModifyZoom(float zoomAmount)
        {
            Zoom = Mathf.Clamp(Zoom - zoomAmount, 1.0f, 45f);
        }

        public void setZoom(float zoom)
        {
            Zoom = zoom;
        }
        public float getZoom()
        {
            return Zoom;
        }
        public void AddZoom(float value)
        {
            Zoom += value;
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            var AspectRatio = (float)RenderSystem.Width / (float)RenderSystem.Height;
            if (Orthographic)
            {
                float zoomedWidth = projectionSize.X * Zoom;
                float zoomedHeight = projectionSize.Y * Zoom;

                // Adjust for aspect ratio to prevent stretching
                float aspectAdjustedWidth = zoomedHeight * AspectRatio;

                projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                    0, aspectAdjustedWidth,
                    0, zoomedHeight,
                    0.5f, 100
                );

                Matrix4x4.Invert(projectionMatrix, out InverseProjMatrix);
                return projectionMatrix;
            }
            return Matrix4x4.CreatePerspectiveFieldOfView(Mathf.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
        }
        public void adjustProjection()
        {
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, 32 * 40, 0, 32 * 21, 0, 100);
        }

        public Matrix4x4 getViewMatrix()
        {
            Vector3 cameraFront = new Vector3(0, 0, -1f);
            Vector3 cameraUp = new Vector3(0, 1, 0f);
            viewMatrix = Matrix4x4.Identity;
            viewMatrix = Matrix4x4.CreateLookAt(new Vector3(Position.X, Position.Y, 0), cameraFront + new Vector3(Position.X, Position.Y, 0), cameraUp);
            Matrix4x4.Invert(viewMatrix, out InverseViewMatrix);

            return viewMatrix;
        }
        public Matrix4x4 GetInverseProj()
        {
            return InverseProjMatrix;
        }
        public Matrix4x4 GetInverseView()
        {
            return InverseViewMatrix;
        }
        public Vector2 getProjectionSize()
        {
            return projectionSize;
        }
    }
}
