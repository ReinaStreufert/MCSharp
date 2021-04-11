using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.Misc.Data
{
    struct Vec2
    {
        public int X;
        public int Y;
        public Vec2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2(float X, float Y)
        {
            this.X = (int)Math.Round(X);
            this.Y = (int)Math.Round(Y);
        }
        public Vec2(float X, float Y, RoundType RoundMethod)
        {
            if (RoundMethod == RoundType.Round)
            {
                this.X = (int)Math.Round(X);
                this.Y = (int)Math.Round(Y);
            } else if (RoundMethod == RoundType.Floor)
            {
                this.X = (int)Math.Floor(X);
                this.Y = (int)Math.Floor(Y);
            } else
            {
                this.X = (int)Math.Ceiling(X);
                this.Y = (int)Math.Ceiling(Y);
            }
        }
    }
    struct Vec2F
    {
        public float X;
        public float Y;
        public Vec2F(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2F(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    struct Vec3
    {
        public int X;
        public int Y;
        public int Z;
        public Vec3(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3(float X, float Y, float Z)
        {
            this.X = (int)Math.Round(X);
            this.Y = (int)Math.Round(Y);
            this.Z = (int)Math.Round(Z);
        }
        public Vec3(float X, float Y, float Z, RoundType RoundMethod)
        {
            if (RoundMethod == RoundType.Round)
            {
                this.X = (int)Math.Round(X);
                this.Y = (int)Math.Round(Y);
                this.Z = (int)Math.Round(Z);
            }
            else if (RoundMethod == RoundType.Floor)
            {
                this.X = (int)Math.Floor(X);
                this.Y = (int)Math.Floor(Y);
                this.Z = (int)Math.Floor(Z);
            }
            else
            {
                this.X = (int)Math.Ceiling(X);
                this.Y = (int)Math.Ceiling(Y);
                this.Z = (int)Math.Ceiling(Z);
            }
        }
    }
    struct Vec3F
    {
        public float X;
        public float Y;
        public float Z;
        public Vec3F(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3F(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
    enum RoundType : byte
    {
        Round,
        Floor,
        Ceiling
    }
}
