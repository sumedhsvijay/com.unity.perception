﻿using Unity.Entities;

namespace UnityEngine.Perception.GroundTruth
{
    /// <summary>
    /// Information regarding a Labeling instance. Generated by <see cref="GroundTruthLabelSetupSystem"/>
    /// </summary>
    public struct GroundTruthInfo : IComponentData
    {
        /// <summary>
        /// The instanceId assigned to the <see cref="Labeling"/>
        /// </summary>
        public uint instanceId;
    }
}
