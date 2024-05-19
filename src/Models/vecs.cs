﻿namespace EQTool.Models
{
    public class vec2
    {
        public float x { get; set; }
        public float y { get; set; }
        public vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public class ivec2
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class vec3
    {
        public vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
