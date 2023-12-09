using GameEngine.components;
using Silk.NET.Maths;
using System;
using System.Numerics;

namespace GameEngine
{
    public class Camera
    {
        public Vector3 Position;// { get { return new Vector3(gameObject.transform.position.X, gameObject.transform.position.Y, 0); } set {gameObject.transform.position.X=value.X;gameObject.transform.position.Y = value.Y; } }
                                //  public Vector2 position { get { return new Vector2(Position.X, position.Y); } set; }
        public float AspectRatio = 1;

        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; }

        //private Vector2 projectionSize = new Vector2(32 * 40, 32 * 21);
        private Vector2 projectionSize = new Vector2(6,3);
        private float Zoom = 1f;
        //private float Zoom = 45f;
        public static Camera Main=null;
        public bool Orthographic=true;
        Matrix4x4 projectionMatrix=new Matrix4x4(); 
        Matrix4x4 viewMatrix=new Matrix4x4(); 
        Matrix4x4 InverseProjMatrix=new Matrix4x4(); 
        Matrix4x4 InverseViewMatrix=new Matrix4x4(); 
     
        
        public Camera(Vector3 position,float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            Main = this;
        }


        public Camera(Vector3 position)
        {
            this.Position = position;   
            Main = this;
        }
        public void ModifyZoom(float zoomAmount)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            Zoom = Mathf.Clamp(Zoom - zoomAmount, 1.0f, 45f);
            
        }

        public void setZoom(float zoom)
        {
            Zoom = zoom;
        }
        public float getZoom()
        {
         return   Zoom;
        }
        public void AddZoom(float value)
        {
            Zoom += value;
        }
     //   public void ModifyDirection(float xOffset, float yOffset)
     //   {
     //       Yaw += xOffset;
     //       Pitch -= yOffset;
     //
     //       //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
     //       Pitch = Mathf.Clamp(Pitch, -89f, 89f);
     //
     //       var cameraDirection = Vector3.Zero;
     //       cameraDirection.X = (float)Math.Cos((double)(Mathf.DegreesToRadians(Yaw)) * Math.Cos((double)Mathf.DegreesToRadians(Pitch)));
     //       cameraDirection.Y = (float)Math.Sin((double)(Mathf.DegreesToRadians(Pitch)));
     //       cameraDirection.Z = (float)(Math.Sin((double)Mathf.DegreesToRadians(Yaw)) * Math.Cos((double)Mathf.DegreesToRadians(Pitch)));
     //
     //       Front = Vector3.Normalize(cameraDirection);
     //   }
     
        public Matrix4x4 GetProjectionMatrix()
        {
            if (Orthographic)
            {
                
                projectionMatrix= Matrix4x4.CreateOrthographicOffCenter(0, projectionSize.X*Zoom,0,projectionSize.Y*Zoom, 0.5f, 100);
                Matrix4x4.Invert(projectionMatrix,out InverseProjMatrix);
                return projectionMatrix;
             //   return Matrix4x4.CreateOrthographic(Mathf.DegreesToRadians(Zoom+400+100), Mathf.DegreesToRadians(Zoom+400), 0f, 100.0f);
            }
               return Matrix4x4.CreatePerspectiveFieldOfView(Mathf.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
        }

        public void adjustProjection()
        {
            projectionMatrix= Matrix4x4.CreateOrthographicOffCenter(0, 32 * 40, 0, 32 * 21, 0, 100);
        }

        public Matrix4x4 getViewMatrix()
        {
            Vector3 cameraFront = new Vector3(0, 0, -1f);
            Vector3 cameraUp = new Vector3(0, 1, 0f);
            viewMatrix = Matrix4x4.Identity;      
            viewMatrix = Matrix4x4.CreateLookAt(new Vector3(Position.X, Position.Y, 0),cameraFront+new Vector3(Position.X,Position.Y,0),cameraUp);
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
