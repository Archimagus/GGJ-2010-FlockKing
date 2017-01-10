using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FlocKing.Animations
{
    public class SkinningData
    {
        // Example Code From the XNA Tutorial Website
        
            /// <summary>
            /// Constructs a new skinning data object.
            /// </summary>
            public SkinningData(Dictionary<string, AnimationClip> animationClips,
                                List<Matrix> bindPose, List<Matrix> inverseBindPose,
                                List<int> skeletonHierarchy)
            {
                AnimationClips = animationClips;
                BindPose = bindPose;
                InverseBindPose = inverseBindPose;
                SkeletonHierarchy = skeletonHierarchy;
            }


            /// <summary>
            /// Private constructor for use by the XNB deserializer.
            /// </summary>
            private SkinningData()
            {
            }


            /// <summary>
            /// Gets a collection of animation clips. These are stored by name in a
            /// dictionary, so there could for instance be clips for "Walk", "Run",
            /// "JumpReallyHigh", etc.
            /// </summary>
            [ContentSerializer]
            public Dictionary<string, AnimationClip> AnimationClips { get; private set; }


            /// <summary>
            /// Bindpose matrices for each bone in the skeleton,
            /// relative to the parent bone.
            /// </summary>
            [ContentSerializer]
            public List<Matrix> BindPose { get; private set; }


            /// <summary>
            /// Vertex to bonespace transforms for each bone in the skeleton.
            /// </summary>
            [ContentSerializer]
            public List<Matrix> InverseBindPose { get; private set; }


            /// <summary>
            /// For each bone in the skeleton, stores the index of the parent bone.
            /// </summary>
            [ContentSerializer]
            public List<int> SkeletonHierarchy { get; private set; }
        }
    }

