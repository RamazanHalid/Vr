// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_IOS)

using UnityEngine;
using UnityEngine.VR;

/// @cond
namespace Gvr.Internal {
  public class UnityVRDevice : BaseVRDevice {

    public override void Init() { }

    public override void SetVRModeEnabled(bool enabled) {
      UnityEngine.XR.XRSettings.enabled = enabled;
    }

    public override void ShowSettingsDialog() { }
    public override void SetDistortionCorrectionEnabled(bool enabled) { }
    public override void SetNeckModelScale(float scale) { }
    public override void UpdateScreenData() { }
    public override void PostRender(RenderTexture stereoScreen) { }

    public override bool SetDefaultDeviceProfile(System.Uri uri) {
      return false;
    }

    // Implemented only for bridging to the GVR native integration.
    public override void UpdateState() {
      this.headPose.Set(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head),
          UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head));
    }

    public override void Recenter() {
      UnityEngine.XR.InputTracking.Recenter();
    }

    public override void OnPause(bool pause) { }
    public override void OnApplicationQuit() { }

    private void SetApplicationState() { }
  }
}
/// @endcond

#endif  // UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_IOS)
