using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Mathf
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
        public static float DegreesToRadians(float degrees)
        {
            return (float)((Math.PI / 180f) * degrees);
        }
        public static float ToDegrees(float degrees)
        {
            return (float)((180f / Math.PI) * degrees);
        }
        public static float Clamp01(float value)
        {
            if (value < 0F)
                return 0F;
            else if (value > 1F)
                return 1F;
            else
                return value;
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            var u = point - origin; //point relative to origin  

            if (u == Vector2.Zero)
                return point;

            var a = (float)Math.Atan2(u.Y, u.X); //angle relative to origin  
            a += rotation; //rotate  

            double radians = rotation * Math.PI / 180;

            //float x = (float)  Math.Cos(radians) * u.Length() + u.X;
            //float y =  (float) Math.Sin(radians) * u.Length()+ u.Y;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            float x = (float)(u.X * cos - u.Y * sin);
            float y = (float)(u.Y * cos + u.X * sin);

            u = new Vector2(x, y);

            //u is now the new point relative to origin  
            //     u = u.Length() * new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            return u + origin;
            return u;
        }


        public static Matrix4x4 translation(float x, float y, float z)
        {
            Matrix4x4 m = new Matrix4x4();
            m.M11 = m.M22 = m.M33 = 1;
            m.M14 = x;
            m.M24 = y;
            m.M34 = z;
            return Matrix4x4.CreateTranslation(x, y, z);
            return m;
        }


        public static Matrix4x4 scale(Vector3 vec, Matrix4x4 src, ref Matrix4x4 dest)
        {
            // Matrix4x4    dest = new Matrix4x4();
            dest.M11 = src.M11 * vec.X;
            dest.M12 = src.M12 * vec.X;
            dest.M13 = src.M13 * vec.X;
            dest.M14 = src.M14 * vec.X;
            dest.M21 = src.M21 * vec.Y;
            dest.M22 = src.M22 * vec.Y;
            dest.M23 = src.M23 * vec.Y;
            dest.M24 = src.M24 * vec.Y;
            dest.M31 = src.M31 * vec.Z;
            dest.M32 = src.M32 * vec.Z;
            dest.M33 = src.M33 * vec.Z;
            dest.M34 = src.M34 * vec.Z;
            return dest;
        }


        public static Matrix4x4 Rotate(float angle, Vector3 axis, Matrix4x4 src, ref Matrix4x4 dest)
        {
            // if (dest == null)
            // Matrix4x4   dest = new Matrix4x4();
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);
            float oneminusc = 1.0f - c;
            float xy = axis.X * axis.Y;
            float yz = axis.Y * axis.Z;
            float xz = axis.X * axis.Z;
            float xs = axis.X * s;
            float ys = axis.Y * s;
            float zs = axis.Z * s;

            float f00 = axis.X * axis.X * oneminusc + c;
            float f01 = xy * oneminusc + zs;
            float f02 = xz * oneminusc - ys;
            // n[3] not used
            float f10 = xy * oneminusc - zs;
            float f11 = axis.Y * axis.Y * oneminusc + c;
            float f12 = yz * oneminusc + xs;
            // n[7] not used
            float f20 = xz * oneminusc + ys;
            float f21 = yz * oneminusc - xs;
            float f22 = axis.Z * axis.Z * oneminusc + c;

            float t00 = src.M11 * f00 + src.M21 * f01 + src.M31 * f02;
            float t01 = src.M12 * f00 + src.M22 * f01 + src.M32 * f02;
            float t02 = src.M13 * f00 + src.M23 * f01 + src.M33 * f02;
            float t03 = src.M14 * f00 + src.M24 * f01 + src.M34 * f02;
            float t10 = src.M11 * f10 + src.M21 * f11 + src.M31 * f12;
            float t11 = src.M12 * f10 + src.M22 * f11 + src.M32 * f12;
            float t12 = src.M13 * f10 + src.M23 * f11 + src.M33 * f12;
            float t13 = src.M14 * f10 + src.M24 * f11 + src.M34 * f12;
            dest.M31 = src.M11 * f20 + src.M21 * f21 + src.M31 * f22;
            dest.M32 = src.M12 * f20 + src.M22 * f21 + src.M32 * f22;
            dest.M33 = src.M13 * f20 + src.M23 * f21 + src.M33 * f22;
            dest.M34 = src.M14 * f20 + src.M24 * f21 + src.M34 * f22;
            dest.M11 = t00;
            dest.M12 = t01;
            dest.M13 = t02;
            dest.M14 = t03;
            dest.M21 = t10;
            dest.M22 = t11;
            dest.M23 = t12;
            dest.M24 = t13;
            return dest;
        }

        public static Matrix4x4 translate(Vector3 vec, Matrix4x4 src, ref Matrix4x4 dest)
        {
            //  Matrix4x4 dest = new Matrix4x4();

            dest.M41 += src.M11 * vec.X + src.M21 * vec.Y + src.M31 * vec.Z;
            dest.M42 += src.M12 * vec.X + src.M22 * vec.Y + src.M32 * vec.Z;
            dest.M43 += src.M13 * vec.X + src.M23 * vec.Y + src.M33 * vec.Z;
            dest.M44 += src.M14 * vec.X + src.M24 * vec.Y + src.M34 * vec.Z;

            return dest;
        }

        public static Vector4 Multiply(Matrix4x4 matrix, Vector4 vector)
        {
            return new Vector4(
                matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z + matrix.M41 * vector.W,
                matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z + matrix.M42 * vector.W,
                matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z + matrix.M43 * vector.W,
                matrix.M14 * vector.X + matrix.M24 * vector.Y + matrix.M34 * vector.Z + matrix.M44 * vector.W
                );
        }

        public static Matrix4x4 rotate(Matrix4x4 m, float x, float y, float z, float theta)
        {
            //   Matrix4x4 m = new Matrix4x4();
            float invLen = 1 / (float)Math.Sqrt(x * x + y * y + z * z);
            x *= invLen;
            y *= invLen;
            z *= invLen;
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);
            float t = 1 - c;
            m.M11 += t * x * x + c;
            m.M22 += t * y * y + c;
            m.M33 += t * z * z + c;
            float txy = t * x * y;
            float sz = s * z;
            m.M12 += txy - sz;
            m.M21 += txy + sz;
            float txz = t * x * z;
            float sy = s * y;
            m.M13 += txz + sy;
            m.M31 += txz - sy;
            float tyz = t * y * z;
            float sx = s * x;
            m.M23 += tyz - sx;
            m.M32 += tyz + sx;

            return m;
        }


    }

}
