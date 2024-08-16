using Basis.Scripts.BasisSdk.Players;
using Basis.Scripts.TransformBinders.BoneControl;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Basis.Scripts.Device_Management.Devices.Simulation
{
    public class BasisSimulateXR : BasisBaseTypeManagement
    {
        public List<BasisInputXRSimulate> Inputs = new List<BasisInputXRSimulate>();
        public async Task<BasisInputXRSimulate> CreatePhysicalTrackedDevice(string UniqueID, string UnUniqueID, BasisBoneTrackedRole Role = BasisBoneTrackedRole.LeftHand, bool hasrole = false, string subSystems = "BasisSimulateXR")
        {
            GameObject gameObject = new GameObject(UniqueID);
            gameObject.transform.parent = BasisLocalPlayer.Instance.LocalBoneDriver.transform;

            GameObject Moveable = new GameObject(UniqueID + " move transform");
            Moveable.transform.parent = BasisLocalPlayer.Instance.LocalBoneDriver.transform;

            BasisInputXRSimulate BasisInput = gameObject.AddComponent<BasisInputXRSimulate>();
            BasisInput.FollowMovement = Moveable.transform;
            await BasisInput.InitalizeTracking(UniqueID, UnUniqueID, subSystems, hasrole, Role);
            if (Inputs.Contains(BasisInput) == false)
            {
                Inputs.Add(BasisInput);
            }
            BasisDeviceManagement.Instance.TryAdd(BasisInput);
            return BasisInput;
        }

        public override void StopSDK()
        {
        }

        public override async Task BeginLoadSDK()
        {
        }

        public override async Task StartSDK()
        {
        }

        public override string Type()
        {
            return "SimulateXR";
        }
    }
}