using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FlocKing.Objects
{
    public class World : ThreeDRenderable
    {
        public BoundingBox WorldBounds { get; set; }
        public List<BoundingSphere> CollisionObjects { get; set; }
        public Vector3 PlayerStart1 { get; set; }
        public Vector3 PlayerStart2 { get; set; }
        public Vector3 PlayerStart3 { get; set; }
        public Vector3 PlayerStart4 { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraTarget { get; set; }
        public bool HasCameraData { get; set; }
        public bool HasPlayerData { get; set; }
        public bool HasWorldBounds { get; set; }
        string _mapName;

        public World(string mapName)
            :base()
        {
            _mapName = mapName;
            CollisionObjects = new List<BoundingSphere>();
            HasCameraData = false;
        }
        public override void Loadcontent()
        {
            Model = Content.Load<Model>("mesh\\marMapv2");
            ModelMesh bounds;
            if (Model.Meshes.TryGetValue("worldBounds", out bounds))
            {
                int numVerts = 0;
                foreach(var part in bounds.MeshParts)
                {
                    numVerts += part.NumVertices;
                }
                Vector3[] points = new Vector3[numVerts];
                bounds.VertexBuffer.GetData(points);
                WorldBounds = BoundingBox.CreateFromPoints(points);
                HasWorldBounds = true;
            }

            foreach (var mesh in Model.Meshes)
            {
                if(mesh.Name.Contains("colObj"))
                {
                    BoundingSphere bs = mesh.BoundingSphere;
                    bs.Center = mesh.ParentBone.Transform.Translation;
                    bs.Center.Y = 0;
                    CollisionObjects.Add(bs);
                }
            }

            ModelMesh playerStart;
            HasCameraData = Model.Meshes.TryGetValue("spawnPoint01", out playerStart);
            if (HasCameraData)
                PlayerStart1 = playerStart.ParentBone.Transform.Translation;
            HasCameraData &= Model.Meshes.TryGetValue("spawnPoint02", out playerStart);
            if (HasCameraData)
                PlayerStart2 = playerStart.ParentBone.Transform.Translation;
            HasCameraData &= Model.Meshes.TryGetValue("spawnPoint03", out playerStart);
            if (HasCameraData)
                PlayerStart3 = playerStart.ParentBone.Transform.Translation;
            HasCameraData &= Model.Meshes.TryGetValue("spawnPoint04", out playerStart);
            if (HasCameraData)
                PlayerStart4 = playerStart.ParentBone.Transform.Translation;

            ModelMesh camBox;
            HasCameraData = Model.Meshes.TryGetValue("camBox", out camBox);
            if (HasCameraData)
                CameraPosition = camBox.ParentBone.Transform.Translation;
            HasCameraData &= Model.Meshes.TryGetValue("camTarget", out camBox);
            if (HasCameraData)
                CameraTarget = camBox.ParentBone.Transform.Translation;

            Color = Color.White;
        }
    }
}
