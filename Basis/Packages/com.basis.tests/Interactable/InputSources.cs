﻿using System;
using System.Linq;
using Basis.Scripts.Device_Management.Devices;
using Basis.Scripts.TransformBinders.BoneControl;

public abstract partial class InteractableObject
{
    [Serializable]
    public struct InputSources {
        public BasisInputWrapper desktopCenterEye, leftHand, rightHand;

        private BasisInputWrapper[] primary; // scratch array to avoid alloc on ToArray
        public BasisInputWrapper[] extras;
        
        public InputSources(uint extrasCount)
        {
            desktopCenterEye = default;
            leftHand = default;
            rightHand = default;
            extras = new BasisInputWrapper[extrasCount];
            primary = new BasisInputWrapper[3];
        }

        private static bool IsInfluencing(InteractInputState state)
        {
            return state == InteractInputState.Hovering || state == InteractInputState.Interacting;
        }

        public readonly bool AnyInteracting(bool skipExtras = true)
        {
            bool influencing = IsInfluencing(desktopCenterEye.GetState()) ||
                            IsInfluencing(leftHand.GetState()) ||
                            IsInfluencing(rightHand.GetState());
            if (!skipExtras)
            {
                influencing |= extras.Any(x => IsInfluencing(x.GetState()));
            }
            return influencing;
        }

        public readonly BasisInputWrapper? FindExcludeExtras(BasisInput input)
        {
            if (input == null)
                return null;
            // done this way to avoid the array GC alloc
            var inUDI = input.UniqueDeviceIdentifier;
            if (desktopCenterEye.GetState() != InteractInputState.NotAdded && desktopCenterEye.Source.UniqueDeviceIdentifier == inUDI)
            {
                return desktopCenterEye;
            }
            else if (leftHand.GetState() != InteractInputState.NotAdded && leftHand.Source.UniqueDeviceIdentifier == inUDI)
            {
                return leftHand;
            }
            else if (rightHand.GetState() != InteractInputState.NotAdded && rightHand.Source.UniqueDeviceIdentifier == inUDI)
            {
                return rightHand;
            }

            return null;
        }


        public readonly bool IsInputAdded(BasisInput input, bool skipExtras = true)
        {
            if (input == null)
                return false;
            string inUDI = input.UniqueDeviceIdentifier;

            bool contains = leftHand.GetState() != InteractInputState.NotAdded && leftHand.Source.UniqueDeviceIdentifier == inUDI || 
                            rightHand.GetState() != InteractInputState.NotAdded && rightHand.Source.UniqueDeviceIdentifier == inUDI || 
                            desktopCenterEye.GetState() != InteractInputState.NotAdded && desktopCenterEye.Source.UniqueDeviceIdentifier == inUDI;

            if (!skipExtras)
            {
                contains |= extras.Any(x => x.GetState() != InteractInputState.NotAdded && x.Source.UniqueDeviceIdentifier == inUDI);
            }
            return contains;
        }

        public readonly BasisInputWrapper[] ToArray()
        {
            primary[0] = desktopCenterEye;
            primary[1] = leftHand;
            primary[2] = rightHand;

            if (extras.Length != 0)
                return primary.Concat(extras).ToArray();
            return primary;
        }

        public bool SetInputByRole(BasisInput input, InteractInputState state)
        {
            var created = BasisInputWrapper.TryNewTracking(input, state, out BasisInputWrapper wrapper);
            if (!created)
                return false;

            switch (wrapper.Role)
            {    
                case BasisBoneTrackedRole.CenterEye:
                    desktopCenterEye = wrapper;
                    return true;
                case BasisBoneTrackedRole.LeftHand:
                    leftHand = wrapper;
                    return true;
                case BasisBoneTrackedRole.RightHand:
                    rightHand = wrapper;
                    return true;
                default:
                    return false;
            }
        }
        public readonly bool TryGetByRole(BasisBoneTrackedRole role, out BasisInputWrapper input)
        {
            input = default;
            switch (role)
            {
                case BasisBoneTrackedRole.CenterEye:
                    input = desktopCenterEye;
                    return true;
                case BasisBoneTrackedRole.LeftHand:
                    input = leftHand;
                    return true;
                case BasisBoneTrackedRole.RightHand:
                    input = rightHand;
                    return true;
                default:
                    return false;
            }
        }

        public bool ChangeStateByRole(BasisBoneTrackedRole role, InteractInputState newState)
        {
            switch (role)
            {
                case BasisBoneTrackedRole.CenterEye:
                    return desktopCenterEye.TrySetState(newState);
                case BasisBoneTrackedRole.LeftHand:
                    return leftHand.TrySetState(newState);
                case BasisBoneTrackedRole.RightHand:
                    return rightHand.TrySetState(newState);
                default:
                    return false;
            }
        }

        public bool RemoveByRole(BasisBoneTrackedRole role)
        {
            switch (role)
            {
                case BasisBoneTrackedRole.CenterEye:
                    desktopCenterEye = default;
                    return true;
                case BasisBoneTrackedRole.LeftHand:
                    leftHand = default;
                    return true;
                case BasisBoneTrackedRole.RightHand:
                    rightHand = default;
                    return true;
                default:
                    return false;
            }
        }
    }
}
