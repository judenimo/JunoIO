using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public sealed class TelemetryState
    {
        public float altitude_asl;
        public float altitude_agl;
        public float altitude_terrain;
        public float mass;
        public float dry_mass;
        public float fuel_mass_scaled;
        public float remaining_fuel_in_stage_fraction;
        public float remaining_monopropellant_fraction;
        public float remaining_battery_fraction;
        public float surface_speed;
        public float horizontal_speed;
        public float vertical_speed;
        public float orbital_speed;
        public float pitch;
        public float bank_angle;
        public float heading;
        public float angle_of_attack;
        public float side_slip;
        public float latitude_deg;
        public float longitude_deg;
        public float distance_from_center;
        public float acceleration;
        public float drag_acceleration;
        public float g_force;
        public float angular_speed;
        public float current_engine_thrust;
        public float current_rcs_thrust_scaled;
        public int active_engine_count;
        public int active_rcs_count;
        public bool grounded;
        public bool in_water;
        public bool supports_warp_burn;
        public float mach_number;
        public float current_isp;
        public float delta_v_stage;
        public float thrust_to_weight_ratio;
        public float remaining_burn_time;
        public float fuel_all_stages_fraction;
        public float weighted_throttle_response;
        public float weighted_throttle_response_time;
        public string planet_name;
        public float planet_radius;
        public float planet_surface_gravity;
        public float position_x;
        public float position_y;
        public float position_z;
        public float velocity_x;
        public float velocity_y;
        public float velocity_z;
        public float surface_velocity_x;
        public float surface_velocity_y;
        public float surface_velocity_z;
        public float target_velocity_x;
        public float target_velocity_y;
        public float target_velocity_z;
        public float acceleration_x;
        public float acceleration_y;
        public float acceleration_z;
        public float angular_velocity_x;
        public float angular_velocity_y;
        public float angular_velocity_z;
        public float craft_forward_x;
        public float craft_forward_y;
        public float craft_forward_z;
        public float craft_right_x;
        public float craft_right_y;
        public float craft_right_z;
        public float craft_up_x;
        public float craft_up_y;
        public float craft_up_z;
        public float nav_north_x;
        public float nav_north_y;
        public float nav_north_z;
        public float nav_east_x;
        public float nav_east_y;
        public float nav_east_z;
        public float nav_up_x;
        public float nav_up_y;
        public float nav_up_z;
        public float nav_craft_roll_axis_x;
        public float nav_craft_roll_axis_y;
        public float nav_craft_roll_axis_z;
        public float nav_craft_pitch_axis_x;
        public float nav_craft_pitch_axis_y;
        public float nav_craft_pitch_axis_z;
        public float nav_craft_yaw_axis_x;
        public float nav_craft_yaw_axis_y;
        public float nav_craft_yaw_axis_z;
        public float body_velocity_forward;
        public float body_velocity_right;
        public float body_velocity_up;
        public float surface_body_velocity_forward;
        public float surface_body_velocity_right;
        public float surface_body_velocity_up;
        public float control_throttle;
        public float control_pitch;
        public float control_roll;
        public float control_yaw;
        public float control_brake;
        public float control_translate_forward;
        public float control_translate_right;
        public float control_translate_up;
        public bool control_translation_mode_enabled;
        public float control_slider_1;
        public float control_slider_2;
        public float control_slider_3;
        public float control_slider_4;
        public bool control_pitch_input_received;
        public bool control_roll_input_received;
        public bool control_yaw_input_received;
        public float environment_air_density;
        public float environment_air_pressure;
        public float environment_temperature;
        public float environment_speed_of_sound;
        public float environment_atmosphere_height;
        public float environment_sample_altitude;
        public float environment_surface_air_density;
        public float gravity_acceleration;
        public float gravity_acceleration_x;
        public float gravity_acceleration_y;
        public float gravity_acceleration_z;
        public float engine_force_x;
        public float engine_force_y;
        public float engine_force_z;
        public float lift_force_x;
        public float lift_force_y;
        public float lift_force_z;
        public float drag_force_x;
        public float drag_force_y;
        public float drag_force_z;
        public float solar_radiation_intensity;
        public float max_engine_thrust;
        public float orbit_apoapsis_altitude;
        public float orbit_apoapsis_time;
        public float orbit_periapsis_altitude;
        public float orbit_periapsis_time;
        public float orbit_eccentricity;
        public float orbit_inclination_rad;
        public float orbit_period;
        public float frame_delta_time;
        public float time_since_launch;
        public float total_time;
        public float warp_amount;
        public float real_time;
    }

    [Serializable]
    public sealed class TelemetryVector3
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public sealed class TelemetryBodyRef
    {
        public string body_id;
        public string name;
    }

    [Serializable]
    public sealed class TelemetryTarget
    {
        public string target_type;
        public string target_id;
        public string name;
        public TelemetryBodyRef parent_body;
        public TelemetryVector3 position;
        public TelemetryVector3 velocity;
        public string craft_id;
        public int? part_id;
        public string part_name;
        public string parent_craft_id;
        public string parent_craft_name;
        public string body_id;
        public float? radius;
        public float? mass;
        public float? mu;
        public float? angular_velocity;
        public string landmark_id;
        public float? latitude_deg;
        public float? longitude_deg;
    }

    [Serializable]
    public sealed class TelemetryEvent
    {
        public string event_type;
        public string event_id;
        public float time;
        public string craft_id;
        public string craft_name;
        public string sender_craft_id;
        public string sender_craft_name;
        public string other_craft_id;
        public string other_craft_name;
        public int? part_id;
        public string part_name;
        public int? other_part_id;
        public string other_part_name;
        public string message_name;
        public string message_data_json;
        public string delivery_scope;
        public TelemetryBodyRef parent_body;
        public TelemetryBodyRef new_parent_body;
        public float? relative_velocity;
        public float? impulse;
        public bool? is_ground_collision;
        public TelemetryVector3 point;
        public TelemetryVector3 normal;
    }

    internal sealed class PendingMessageCommand
    {
        public string scope;
        public string message;
        public string craft_id;
        public string data_json;
        public float? radius;
    }

    public sealed class JunoIoBridge : MonoBehaviour
    {
        private const string Host = "127.0.0.1";
        private const int TelemetryPort = 5005;
        private const int CommandPort = 5006;
        private const int RpcRequestPort = 5007;
        private const int RpcResponsePort = 5008;
        private const float SendIntervalSeconds = 0.05f;
        private const float CommandTimeoutSeconds = 0.5f;
        private const float DefaultNearbyMessageRadius = 1000f;
        private static readonly FieldInfo[] TelemetryStateFields = typeof(TelemetryState).GetFields(BindingFlags.Instance | BindingFlags.Public);
        private static readonly FieldInfo ForceNozzleTransformField = typeof(Assets.Scripts.Craft.Parts.Modifiers.Propulsion.EngineNozzleScript)
            .GetField("_forceNozzleTransform", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly string WingModifierTypeName = "UnityFS.Wing";
        private static readonly string PropellerModifierTypeName = "Assets.Scripts.Craft.Parts.Modifiers.Propulsion.Propeller.PropellerScript";
        private static readonly string FlightProgramDataTypeName = "Assets.Scripts.Craft.Parts.Modifiers.FlightProgramData";
        private static readonly string PositionTargetTypeName = "Assets.Scripts.Vizzy.Craft.PositionTarget";
        private static readonly string PartTargetTypeName = "Assets.Scripts.Craft.Parts.PartScript";

        private UdpClient _telemetryClient;
        private UdpClient _commandClient;
        private UdpClient _rpcRequestClient;
        private UdpClient _rpcResponseClient;
        private float _nextSendTime;
        private float _lastValidCommandTime = float.NegativeInfinity;
        private float _commandedThrottle;
        private float _commandedPitch;
        private float _commandedRoll;
        private float _commandedYaw;
        private float _commandedBrake;
        private float _commandedTranslateForward;
        private float _commandedTranslateRight;
        private float _commandedTranslateUp;
        private float _commandedSlider1;
        private float _commandedSlider2;
        private float _commandedSlider3;
        private float _commandedSlider4;
        private bool _hasCommandedThrottle;
        private bool _hasCommandedPitch;
        private bool _hasCommandedRoll;
        private bool _hasCommandedYaw;
        private bool _hasCommandedBrake;
        private bool _hasCommandedTranslateForward;
        private bool _hasCommandedTranslateRight;
        private bool _hasCommandedTranslateUp;
        private bool _hasCommandedSlider1;
        private bool _hasCommandedSlider2;
        private bool _hasCommandedSlider3;
        private bool _hasCommandedSlider4;
        private bool _commandedTranslationModeEnabled;
        private bool _hasCommandedTranslationModeEnabled;
        private int _telemetrySequence;
        private int _lastCommandSequence = -1;
        private int _dynamicFlightLogEntryId = -1;
        private int _eventSequence;
        private readonly Queue<TelemetryEvent> _pendingEvents = new Queue<TelemetryEvent>();
        private readonly List<TelemetryEvent> _pendingCraftMessageEvents = new List<TelemetryEvent>();
        private readonly Dictionary<int, bool> _observedDestroyedParts = new Dictionary<int, bool>();
        private readonly HashSet<string> _activeCollisionKeys = new HashSet<string>();
        private ModApi.Craft.ICraftScript _subscribedCraftScript;
        private ModApi.Flight.IFlightScene _subscribedFlightScene;
        private static readonly HashSet<int> TrackedCraftNodeIds = new HashSet<int>();

        private void OnEnable()
        {
            try
            {
                _telemetryClient = new UdpClient();
                _telemetryClient.Connect(Host, TelemetryPort);

                _commandClient = new UdpClient(new IPEndPoint(IPAddress.Parse(Host), CommandPort));
                _commandClient.Client.Blocking = false;

                _rpcRequestClient = new UdpClient(new IPEndPoint(IPAddress.Parse(Host), RpcRequestPort));
                _rpcRequestClient.Client.Blocking = false;

                _rpcResponseClient = new UdpClient();
                _rpcResponseClient.Connect(Host, RpcResponsePort);

                _nextSendTime = Time.time;

                Debug.Log(
                    $"JunoIoBridge started: telemetry {Host}:{TelemetryPort}, commands {Host}:{CommandPort}, rpc {Host}:{RpcRequestPort}->{RpcResponsePort}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"JunoIoBridge failed to start: {ex.Message}");
                DisposeClients();
            }
        }

        private void Update()
        {
            PollRpcRequests();
            PollControlCommands();

            var activeCraftNode = ModApi.Common.Game.Instance.FlightScene?.CraftNode;
            var craftScript = activeCraftNode?.CraftScript;
            RefreshEventSubscriptions(ModApi.Common.Game.Instance.FlightScene, craftScript);
            PollActiveCraftEvents(craftScript);
            var hasFreshCommand = Time.time - _lastValidCommandTime <= CommandTimeoutSeconds;
            if (craftScript?.ActiveCommandPod != null && hasFreshCommand)
            {
                if (_hasCommandedThrottle)
                {
                    craftScript.ActiveCommandPod.Controls.Throttle = _commandedThrottle;
                }

                if (_hasCommandedPitch)
                {
                    craftScript.ActiveCommandPod.Controls.Pitch = _commandedPitch;
                    craftScript.ActiveCommandPod.Controls.OffsetPitch = _commandedPitch;
                }

                if (_hasCommandedRoll)
                {
                    craftScript.ActiveCommandPod.Controls.Roll = _commandedRoll;
                    craftScript.ActiveCommandPod.Controls.OffsetRoll = _commandedRoll;
                }

                if (_hasCommandedYaw)
                {
                    craftScript.ActiveCommandPod.Controls.Yaw = _commandedYaw;
                    craftScript.ActiveCommandPod.Controls.OffsetYaw = _commandedYaw;
                }

                if (_hasCommandedBrake)
                {
                    craftScript.ActiveCommandPod.Controls.Brake = _commandedBrake;
                    craftScript.ActiveCommandPod.Controls.OffsetBrake = _commandedBrake;
                }

                if (_hasCommandedTranslateForward)
                {
                    craftScript.ActiveCommandPod.Controls.TranslateForward = _commandedTranslateForward;
                    craftScript.ActiveCommandPod.Controls.OffsetTranslateForward = _commandedTranslateForward;
                }

                if (_hasCommandedTranslateRight)
                {
                    craftScript.ActiveCommandPod.Controls.TranslateRight = _commandedTranslateRight;
                    craftScript.ActiveCommandPod.Controls.OffsetTranslateRight = _commandedTranslateRight;
                }

                if (_hasCommandedTranslateUp)
                {
                    craftScript.ActiveCommandPod.Controls.TranslateUp = _commandedTranslateUp;
                    craftScript.ActiveCommandPod.Controls.OffsetTranslateUp = _commandedTranslateUp;
                }

                if (_hasCommandedSlider1)
                {
                    craftScript.ActiveCommandPod.Controls.Slider1 = _commandedSlider1;
                    craftScript.ActiveCommandPod.Controls.OffsetSlider1 = _commandedSlider1;
                }

                if (_hasCommandedSlider2)
                {
                    craftScript.ActiveCommandPod.Controls.Slider2 = _commandedSlider2;
                    craftScript.ActiveCommandPod.Controls.OffsetSlider2 = _commandedSlider2;
                }

                if (_hasCommandedSlider3)
                {
                    craftScript.ActiveCommandPod.Controls.Slider3 = _commandedSlider3;
                    craftScript.ActiveCommandPod.Controls.OffsetSlider3 = _commandedSlider3;
                }

                if (_hasCommandedSlider4)
                {
                    craftScript.ActiveCommandPod.Controls.Slider4 = _commandedSlider4;
                    craftScript.ActiveCommandPod.Controls.OffsetSlider4 = _commandedSlider4;
                }

                if (_hasCommandedTranslationModeEnabled)
                {
                    craftScript.ActiveCommandPod.Controls.TranslationModeEnabled = _commandedTranslationModeEnabled;
                }
            }

            if (_telemetryClient == null || Time.time < _nextSendTime || activeCraftNode == null || craftScript == null)
            {
                return;
            }

            var json = SerializeTelemetryEnvelope(activeCraftNode);
            var payload = Encoding.UTF8.GetBytes(json);

            try
            {
                _telemetryClient.Send(payload, payload.Length);
                _nextSendTime += SendIntervalSeconds;

                if (_nextSendTime < Time.time)
                {
                    _nextSendTime = Time.time + SendIntervalSeconds;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"JunoIoBridge UDP send failed: {ex.Message}");
                DisposeClients();
            }
        }

        private void OnDisable()
        {
            DisposeClients();
        }

        private void OnDestroy()
        {
            DisposeClients();
        }

        private void PollControlCommands()
        {
            if (_commandClient == null)
            {
                return;
            }

            try
            {
                while (_commandClient.Available > 0)
                {
                    var endpoint = new IPEndPoint(IPAddress.Any, 0);
                    var payload = _commandClient.Receive(ref endpoint);
                    var json = Encoding.UTF8.GetString(payload);

                    if (!json.Contains("\"control\""))
                    {
                        continue;
                    }

                    if (!TryParseControlMessage(
                            json,
                            out var commandSequence,
                            out var hasThrottle,
                            out var throttle,
                            out var hasPitch,
                            out var pitch,
                            out var hasRoll,
                            out var roll,
                            out var hasYaw,
                            out var yaw,
                            out var hasBrake,
                            out var brake,
                            out var hasTranslateForward,
                            out var translateForward,
                            out var hasTranslateRight,
                            out var translateRight,
                            out var hasTranslateUp,
                            out var translateUp,
                            out var hasSlider1,
                            out var slider1,
                            out var hasSlider2,
                            out var slider2,
                            out var hasSlider3,
                            out var slider3,
                            out var hasSlider4,
                            out var slider4,
                            out var hasTranslationModeEnabled,
                            out var translationModeEnabled))
                    {
                        continue;
                    }

                    var hasAxisCommand = false;
                    if (hasThrottle)
                    {
                        _commandedThrottle = Mathf.Clamp01(throttle);
                        _hasCommandedThrottle = true;
                        hasAxisCommand = true;
                    }

                    if (hasPitch)
                    {
                        _commandedPitch = Mathf.Clamp(pitch, -1f, 1f);
                        _hasCommandedPitch = true;
                        hasAxisCommand = true;
                    }

                    if (hasRoll)
                    {
                        _commandedRoll = Mathf.Clamp(roll, -1f, 1f);
                        _hasCommandedRoll = true;
                        hasAxisCommand = true;
                    }

                    if (hasYaw)
                    {
                        _commandedYaw = Mathf.Clamp(yaw, -1f, 1f);
                        _hasCommandedYaw = true;
                        hasAxisCommand = true;
                    }

                    if (hasBrake)
                    {
                        _commandedBrake = Mathf.Clamp01(brake);
                        _hasCommandedBrake = true;
                        hasAxisCommand = true;
                    }

                    if (hasTranslateForward)
                    {
                        _commandedTranslateForward = Mathf.Clamp(translateForward, -1f, 1f);
                        _hasCommandedTranslateForward = true;
                        hasAxisCommand = true;
                    }

                    if (hasTranslateRight)
                    {
                        _commandedTranslateRight = Mathf.Clamp(translateRight, -1f, 1f);
                        _hasCommandedTranslateRight = true;
                        hasAxisCommand = true;
                    }

                    if (hasTranslateUp)
                    {
                        _commandedTranslateUp = Mathf.Clamp(translateUp, -1f, 1f);
                        _hasCommandedTranslateUp = true;
                        hasAxisCommand = true;
                    }

                    if (hasSlider1)
                    {
                        _commandedSlider1 = Mathf.Clamp(slider1, -1f, 1f);
                        _hasCommandedSlider1 = true;
                        hasAxisCommand = true;
                    }

                    if (hasSlider2)
                    {
                        _commandedSlider2 = Mathf.Clamp(slider2, -1f, 1f);
                        _hasCommandedSlider2 = true;
                        hasAxisCommand = true;
                    }

                    if (hasSlider3)
                    {
                        _commandedSlider3 = Mathf.Clamp(slider3, -1f, 1f);
                        _hasCommandedSlider3 = true;
                        hasAxisCommand = true;
                    }

                    if (hasSlider4)
                    {
                        _commandedSlider4 = Mathf.Clamp(slider4, -1f, 1f);
                        _hasCommandedSlider4 = true;
                        hasAxisCommand = true;
                    }

                    if (hasTranslationModeEnabled)
                    {
                        _commandedTranslationModeEnabled = translationModeEnabled;
                        _hasCommandedTranslationModeEnabled = true;
                        hasAxisCommand = true;
                    }

                    if (hasAxisCommand)
                    {
                        _lastValidCommandTime = Time.time;
                        _lastCommandSequence = commandSequence;
                    }

                    if (TryParseDisplayMessage(json, out var displayMessage, out var displayDuration))
                    {
                        ShowFlightMessage(displayMessage, displayDuration);
                    }

                    if (TryParseFlightLogMessage(json, out var flightLogMessage, out var replaceFlightLogEntry))
                    {
                        ShowFlightLogMessage(flightLogMessage, replaceFlightLogEntry);
                    }

                    if (TryParseStageActivation(json))
                    {
                        ActivateNextStage();
                    }

                    if (TryParseTimeWarpMode(json, out var timeWarpMode))
                    {
                        SetTimeWarpMode(timeWarpMode);
                    }

                    if (TryParseHeadingLockMode(json, out var headingLockMode))
                    {
                        ApplyHeadingLockMode(headingLockMode);
                    }

                    if (TryParseHeadingLockVector(json, out var headingLockVector))
                    {
                        LockHeadingVector(headingLockVector);
                    }

                    if (TryParseAttitudeTarget(json, out var targetHeading, out var targetPitch))
                    {
                        ApplyAttitudeTarget(targetHeading, targetPitch);
                    }

                    if (TryParseActivationGroupCommand(json, out var activationGroup, out var activationGroupEnabled))
                    {
                        SetActivationGroupState(activationGroup, activationGroupEnabled);
                    }

                    if (TryParseTargetNodeCommand(json, out var targetNodeName))
                    {
                        SetTargetNode(targetNodeName);
                    }

                    if (TryParsePidGainsCommand(json, "\"pid_gains_pitch\"", out var pitchPid))
                    {
                        SetPitchPidGains(pitchPid);
                    }

                    if (TryParsePidGainsCommand(json, "\"pid_gains_roll\"", out var rollPid))
                    {
                        SetRollPidGains(rollPid);
                    }

                    if (TryParseMessageCommands(json, out var messageCommands))
                    {
                        var senderCraftNode = ModApi.Common.Game.Instance.FlightScene?.CraftNode;
                        foreach (var messageCommand in messageCommands)
                        {
                            HandleMessageCommand(messageCommand, senderCraftNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"JunoIoBridge UDP receive failed: {ex.Message}");
            }
        }

        private void PollRpcRequests()
        {
            if (_rpcRequestClient == null || _rpcResponseClient == null)
            {
                return;
            }

            try
            {
                while (_rpcRequestClient.Available > 0)
                {
                    var endpoint = new IPEndPoint(IPAddress.Any, 0);
                    var payload = _rpcRequestClient.Receive(ref endpoint);
                    var json = Encoding.UTF8.GetString(payload);

                    if (!TryParseRpcRequest(json, out var requestId, out var method))
                    {
                        SendRpcResponse(SerializeRpcErrorResponse(0, "malformed_request", "Malformed RPC request."));
                        continue;
                    }

                    var responseJson = HandleRpcRequest(requestId, method, json);
                    SendRpcResponse(responseJson);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"JunoIoBridge RPC receive failed: {ex.Message}");
            }
        }

        private TelemetryState BuildTelemetryState(ModApi.Craft.ICraftNode craftNode)
        {
            if (craftNode?.CraftScript != null)
            {
                return BuildTelemetryState(craftNode.CraftScript);
            }

            return BuildTelemetryStateFromCraftNode(craftNode);
        }

        private TelemetryState BuildTelemetryState(ModApi.Craft.ICraftScript craftScript)
        {
            var flightScene = ModApi.Common.Game.Instance.FlightScene;
            var frameDeltaTime = GetFrameDeltaTime(flightScene);
            var totalTime = GetTotalTimeSeconds(flightScene);
            var warpAmount = GetWarpAmount(flightScene);
            var realTime = GetRealTimeSeconds(craftScript);
            var timeSinceLaunch = GetTimeSinceLaunchSeconds(craftScript, totalTime);

            var flightData = craftScript.FlightData;
            var controls = craftScript.ActiveCommandPod?.Controls;
            var orbit = flightData.Orbit;
            var planet = orbit?.Parent;
            var planetData = planet?.PlanetData;
            var atmosphere = flightData.AtmosphereSample;
            var performance = flightData.Performance;

            var position = flightData.Position;
            var velocity = flightData.Velocity;
            var surfaceVelocity = flightData.SurfaceVelocity;
            var targetVelocity = flightData.NavSphereTarget != null && !flightData.NavSphereTarget.IsDestroyed
                ? flightData.NavSphereTarget.Velocity - velocity
                : new Vector3d(0d, 0d, 0d);
            var acceleration = flightData.Acceleration;
            var angularVelocity = flightData.AngularVelocity;
            var craftForward = flightData.CraftForward;
            var craftRight = flightData.CraftRight;
            var craftUp = flightData.CraftUp;
            var navNorth = flightData.North;
            var navEast = flightData.East;
            var navUp = NormalizeOrZero(position);
            var gravityAccelerationVector = craftScript.GravityForce;
            var engineForceVector = BuildEngineForceVector(craftScript);
            var liftForceVector = BuildLiftForceVector(craftScript);
            var dragForceVector = BuildDragForceVector(craftScript);

            var activeEngineCount = CountItems(flightData.ActiveEngines);
            var activeRcsCount = CountItems(flightData.ActiveReactionControlNozzles);

            var latitudeDeg = 0f;
            var longitudeDeg = 0f;
            if (planet != null)
            {
                planet.GetSurfaceCoordinates(flightData.PositionNormalized, out var latitudeRad, out var longitudeRad);
                latitudeDeg = (float)(latitudeRad * (180.0 / Math.PI));
                longitudeDeg = (float)(longitudeRad * (180.0 / Math.PI));
            }

            return new TelemetryState
            {
                altitude_asl = (float)flightData.AltitudeAboveSeaLevel,
                altitude_agl = (float)flightData.AltitudeAboveGroundLevel,
                altitude_terrain = (float)flightData.AltitudeAboveTerrain,
                mass = (float)flightData.CurrentMassUnscaled,
                dry_mass = (float)(flightData.CurrentMassUnscaled - flightData.FuelMass),
                fuel_mass_scaled = (float)flightData.FuelMass,
                remaining_fuel_in_stage_fraction = (float)flightData.RemainingFuelInStage,
                remaining_monopropellant_fraction = (float)flightData.RemainingMonopropellant,
                remaining_battery_fraction = (float)flightData.RemainingBattery,
                surface_speed = (float)flightData.SurfaceVelocityMagnitude,
                horizontal_speed = (float)flightData.LateralSurfaceVelocity,
                vertical_speed = (float)craftScript.GetVerticalVelocity(),
                orbital_speed = (float)flightData.VelocityMagnitude,
                pitch = (float)flightData.Pitch,
                bank_angle = (float)flightData.BankAngle,
                heading = (float)flightData.Heading,
                angle_of_attack = (float)flightData.AngleOfAttack,
                side_slip = (float)flightData.SideSlip,
                latitude_deg = latitudeDeg,
                longitude_deg = longitudeDeg,
                distance_from_center = (float)position.magnitude,
                acceleration = (float)flightData.AccelerationMagnitude,
                drag_acceleration = (float)flightData.DragAccelerationMagnitude,
                g_force = flightData.GravityMagnitude > float.Epsilon
                    ? (float)(flightData.AccelerationMagnitude / flightData.GravityMagnitude)
                    : 0f,
                angular_speed = (float)flightData.AngularVelocityMagnitude,
                current_engine_thrust = (float)flightData.CurrentEngineThrustUnscaled,
                current_rcs_thrust_scaled = (float)flightData.CurrentReactionControlNozzleThrust,
                active_engine_count = activeEngineCount,
                active_rcs_count = activeRcsCount,
                grounded = flightData.Grounded,
                in_water = flightData.InWater,
                supports_warp_burn = flightData.SupportsWarpBurn,
                mach_number = (float)flightData.MachNumber,
                current_isp = performance != null ? (float)performance.CurrentIsp : 0f,
                delta_v_stage = performance != null ? (float)performance.DeltaVStage : 0f,
                thrust_to_weight_ratio = performance != null ? (float)performance.ThrustToWeightRatio : 0f,
                remaining_burn_time = performance != null ? (float)performance.RemainingBurnTime : 0f,
                fuel_all_stages_fraction = performance != null ? (float)performance.FuelAllStagesPercentage : 0f,
                weighted_throttle_response = (float)flightData.WeightedThrottleResponse,
                weighted_throttle_response_time = (float)flightData.WeightedThrottleResponseTime,
                planet_name = planetData?.Name ?? string.Empty,
                planet_radius = planetData != null ? (float)planetData.Radius : 0f,
                planet_surface_gravity = planetData != null ? (float)planetData.SurfaceGravity : 0f,
                position_x = (float)position.x,
                position_y = (float)position.y,
                position_z = (float)position.z,
                velocity_x = (float)velocity.x,
                velocity_y = (float)velocity.y,
                velocity_z = (float)velocity.z,
                surface_velocity_x = (float)surfaceVelocity.x,
                surface_velocity_y = (float)surfaceVelocity.y,
                surface_velocity_z = (float)surfaceVelocity.z,
                target_velocity_x = (float)targetVelocity.x,
                target_velocity_y = (float)targetVelocity.y,
                target_velocity_z = (float)targetVelocity.z,
                acceleration_x = (float)acceleration.x,
                acceleration_y = (float)acceleration.y,
                acceleration_z = (float)acceleration.z,
                angular_velocity_x = (float)angularVelocity.x,
                angular_velocity_y = (float)angularVelocity.y,
                angular_velocity_z = (float)angularVelocity.z,
                craft_forward_x = (float)craftForward.x,
                craft_forward_y = (float)craftForward.y,
                craft_forward_z = (float)craftForward.z,
                craft_right_x = (float)craftRight.x,
                craft_right_y = (float)craftRight.y,
                craft_right_z = (float)craftRight.z,
                craft_up_x = (float)craftUp.x,
                craft_up_y = (float)craftUp.y,
                craft_up_z = (float)craftUp.z,
                nav_north_x = (float)navNorth.x,
                nav_north_y = (float)navNorth.y,
                nav_north_z = (float)navNorth.z,
                nav_east_x = (float)navEast.x,
                nav_east_y = (float)navEast.y,
                nav_east_z = (float)navEast.z,
                nav_up_x = (float)navUp.x,
                nav_up_y = (float)navUp.y,
                nav_up_z = (float)navUp.z,
                nav_craft_roll_axis_x = (float)craftForward.x,
                nav_craft_roll_axis_y = (float)craftForward.y,
                nav_craft_roll_axis_z = (float)craftForward.z,
                nav_craft_pitch_axis_x = (float)craftRight.x,
                nav_craft_pitch_axis_y = (float)craftRight.y,
                nav_craft_pitch_axis_z = (float)craftRight.z,
                nav_craft_yaw_axis_x = (float)craftUp.x,
                nav_craft_yaw_axis_y = (float)craftUp.y,
                nav_craft_yaw_axis_z = (float)craftUp.z,
                body_velocity_forward = (float)Vector3d.Dot(velocity, craftForward),
                body_velocity_right = (float)Vector3d.Dot(velocity, craftRight),
                body_velocity_up = (float)Vector3d.Dot(velocity, craftUp),
                surface_body_velocity_forward = (float)Vector3d.Dot(surfaceVelocity, craftForward),
                surface_body_velocity_right = (float)Vector3d.Dot(surfaceVelocity, craftRight),
                surface_body_velocity_up = (float)Vector3d.Dot(surfaceVelocity, craftUp),
                control_throttle = controls != null ? controls.Throttle : 0f,
                control_pitch = controls != null ? controls.Pitch : 0f,
                control_roll = controls != null ? controls.Roll : 0f,
                control_yaw = controls != null ? controls.Yaw : 0f,
                control_brake = controls != null ? controls.Brake : 0f,
                control_translate_forward = controls != null ? controls.TranslateForward : 0f,
                control_translate_right = controls != null ? controls.TranslateRight : 0f,
                control_translate_up = controls != null ? controls.TranslateUp : 0f,
                control_translation_mode_enabled = controls != null && controls.TranslationModeEnabled,
                control_slider_1 = controls != null ? controls.Slider1 : 0f,
                control_slider_2 = controls != null ? controls.Slider2 : 0f,
                control_slider_3 = controls != null ? controls.Slider3 : 0f,
                control_slider_4 = controls != null ? controls.Slider4 : 0f,
                control_pitch_input_received = controls != null && controls.PitchInputReceived,
                control_roll_input_received = controls != null && controls.RollInputReceived,
                control_yaw_input_received = controls != null && controls.YawInputReceived,
                environment_air_density = (float)atmosphere.AirDensity,
                environment_air_pressure = (float)atmosphere.AirPressure,
                environment_temperature = (float)atmosphere.Temperature,
                environment_speed_of_sound = (float)atmosphere.SpeedOfSound,
                environment_atmosphere_height = (float)atmosphere.AtmosphereHeight,
                environment_sample_altitude = (float)atmosphere.SampleAltitude,
                environment_surface_air_density = (float)atmosphere.SurfaceAirDensity,
                gravity_acceleration = (float)flightData.GravityMagnitude,
                gravity_acceleration_x = (float)gravityAccelerationVector.x,
                gravity_acceleration_y = (float)gravityAccelerationVector.y,
                gravity_acceleration_z = (float)gravityAccelerationVector.z,
                engine_force_x = (float)engineForceVector.x,
                engine_force_y = (float)engineForceVector.y,
                engine_force_z = (float)engineForceVector.z,
                lift_force_x = (float)liftForceVector.x,
                lift_force_y = (float)liftForceVector.y,
                lift_force_z = (float)liftForceVector.z,
                drag_force_x = (float)dragForceVector.x,
                drag_force_y = (float)dragForceVector.y,
                drag_force_z = (float)dragForceVector.z,
                solar_radiation_intensity = (float)flightData.SolarRadiationIntensity,
                max_engine_thrust = (float)flightData.MaxActiveEngineThrustUnscaled,
                orbit_apoapsis_altitude = orbit != null ? (float)orbit.ApoapsisAltitude : 0f,
                orbit_apoapsis_time = orbit != null ? (float)orbit.ApoapsisTime : 0f,
                orbit_periapsis_altitude = orbit != null ? (float)orbit.PeriapsisAltitude : 0f,
                orbit_periapsis_time = orbit != null ? (float)orbit.PeriapsisTime : 0f,
                orbit_eccentricity = orbit != null ? (float)orbit.Eccentricity : 0f,
                orbit_inclination_rad = orbit != null ? (float)orbit.Inclination : 0f,
                orbit_period = orbit != null ? (float)orbit.Period : 0f,
                frame_delta_time = frameDeltaTime,
                time_since_launch = timeSinceLaunch,
                total_time = totalTime,
                warp_amount = warpAmount,
                real_time = realTime,
            };
        }

        private TelemetryState BuildTelemetryStateFromCraftNode(ModApi.Craft.ICraftNode craftNode)
        {
            if (craftNode == null)
            {
                return new TelemetryState();
            }

            var controls = craftNode.Controls;
            var position = ToVector3d(craftNode.Position);
            var velocity = ToVector3d(craftNode.Velocity);
            var surfaceVelocity = ToVector3d(craftNode.SurfaceVelocity);
            var latLon = craftNode.LatLon;
            var targetVelocity = new Vector3d(0d, 0d, 0d);
            var navUp = NormalizeOrZero(position);
            var navEast = BuildEastFromLatLon(latLon);
            var navNorth = NormalizeOrZero(Vector3d.Cross(navUp, navEast));
            var flightScene = ModApi.Common.Game.Instance.FlightScene;
            var frameDeltaTime = GetFrameDeltaTime(flightScene);
            var totalTime = GetTotalTimeSeconds(flightScene);
            var warpAmount = GetWarpAmount(flightScene);

            return new TelemetryState
            {
                altitude_asl = (float)craftNode.Altitude,
                altitude_agl = (float)craftNode.AltitudeAgl,
                altitude_terrain = (float)craftNode.AltitudeAboveTerrain,
                mass = (float)craftNode.CraftMass,
                dry_mass = (float)craftNode.CraftMass, // For unloaded crafts, dry mass fallback
                surface_speed = (float)surfaceVelocity.magnitude,
                orbital_speed = (float)velocity.magnitude,
                latitude_deg = (float)(latLon.x * (180.0 / Math.PI)),
                longitude_deg = (float)(latLon.y * (180.0 / Math.PI)),
                distance_from_center = (float)position.magnitude,
                grounded = craftNode.InContactWithPlanet,
                in_water = craftNode.InContactWithWater,
                supports_warp_burn = craftNode.CanWarp,
                position_x = (float)position.x,
                position_y = (float)position.y,
                position_z = (float)position.z,
                velocity_x = (float)velocity.x,
                velocity_y = (float)velocity.y,
                velocity_z = (float)velocity.z,
                surface_velocity_x = (float)surfaceVelocity.x,
                surface_velocity_y = (float)surfaceVelocity.y,
                surface_velocity_z = (float)surfaceVelocity.z,
                target_velocity_x = (float)targetVelocity.x,
                target_velocity_y = (float)targetVelocity.y,
                target_velocity_z = (float)targetVelocity.z,
                nav_north_x = (float)navNorth.x,
                nav_north_y = (float)navNorth.y,
                nav_north_z = (float)navNorth.z,
                nav_east_x = (float)navEast.x,
                nav_east_y = (float)navEast.y,
                nav_east_z = (float)navEast.z,
                nav_up_x = (float)navUp.x,
                nav_up_y = (float)navUp.y,
                nav_up_z = (float)navUp.z,
                control_throttle = controls != null ? controls.Throttle : 0f,
                control_pitch = controls != null ? controls.Pitch : 0f,
                control_roll = controls != null ? controls.Roll : 0f,
                control_yaw = controls != null ? controls.Yaw : 0f,
                control_brake = controls != null ? controls.Brake : 0f,
                control_translate_forward = controls != null ? controls.TranslateForward : 0f,
                control_translate_right = controls != null ? controls.TranslateRight : 0f,
                control_translate_up = controls != null ? controls.TranslateUp : 0f,
                control_translation_mode_enabled = controls != null && controls.TranslationModeEnabled,
                control_slider_1 = controls != null ? controls.Slider1 : 0f,
                control_slider_2 = controls != null ? controls.Slider2 : 0f,
                control_slider_3 = controls != null ? controls.Slider3 : 0f,
                control_slider_4 = controls != null ? controls.Slider4 : 0f,
                control_pitch_input_received = controls != null && controls.PitchInputReceived,
                control_roll_input_received = controls != null && controls.RollInputReceived,
                control_yaw_input_received = controls != null && controls.YawInputReceived,
                frame_delta_time = frameDeltaTime,
                time_since_launch = totalTime,
                total_time = totalTime,
                warp_amount = warpAmount,
                real_time = totalTime,
            };
        }

        private TelemetryTarget BuildTelemetryTarget(ModApi.Flight.UI.INavSphereTarget navTarget)
        {
            if (navTarget == null || navTarget.IsDestroyed)
            {
                return null;
            }

            var targetType = ClassifyTargetType(navTarget);
            var position = CreateTelemetryVector3(ToVector3d(navTarget.Position));
            var velocity = CreateTelemetryVector3(ToVector3d(navTarget.Velocity));

            switch (targetType)
            {
                case "craft":
                    return BuildCraftTargetTelemetry(navTarget, position, velocity);
                case "body":
                    return BuildBodyTargetTelemetry(navTarget, position, velocity);
                case "part":
                    return BuildPartTargetTelemetry(navTarget, position, velocity);
                case "landmark":
                    return BuildLandmarkTargetTelemetry(navTarget, position, velocity);
                default:
                    return BuildPositionTargetTelemetry(navTarget, position, velocity);
            }
        }

        private TelemetryTarget BuildCraftTargetTelemetry(
            ModApi.Flight.UI.INavSphereTarget navTarget,
            TelemetryVector3 position,
            TelemetryVector3 velocity)
        {
            var craftNode = GetCraftNodeFromTarget(navTarget);
            var craftId = craftNode != null ? FormatCraftId(craftNode.NodeId) : null;
            var craftName = craftNode != null ? GetCraftName(craftNode) : navTarget.Name;
            return new TelemetryTarget
            {
                target_type = "craft",
                target_id = craftId ?? navTarget.Name ?? "craft_target",
                name = craftName ?? string.Empty,
                parent_body = BuildBodyRef(navTarget.Parent),
                position = position,
                velocity = velocity,
                craft_id = craftId,
            };
        }

        private TelemetryTarget BuildBodyTargetTelemetry(
            ModApi.Flight.UI.INavSphereTarget navTarget,
            TelemetryVector3 position,
            TelemetryVector3 velocity)
        {
            var planetNode = GetPlanetNodeFromTarget(navTarget);
            var planetData = planetNode?.PlanetData;
            var parentBody = planetData?.Parent != null ? BuildBodyRef(planetData.Parent) : BuildBodyRef(navTarget.Parent);
            var radius = planetData != null ? (float?)planetData.Radius : null;
            var mass = planetData != null ? (float?)planetData.Mass : null;
            var mu = planetData != null ? (float?)(planetData.SurfaceGravity * planetData.RadiusSquared) : null;
            var angularVelocity = planetData != null ? (float?)planetData.AngularVelocity : null;
            var bodyId = FormatBodyId(planetData?.Id);
            var bodyName = planetData?.Name ?? navTarget.Name;

            return new TelemetryTarget
            {
                target_type = "body",
                target_id = bodyId ?? bodyName ?? "body_target",
                name = bodyName ?? string.Empty,
                parent_body = parentBody,
                position = position,
                velocity = velocity,
                body_id = bodyId,
                radius = radius,
                mass = mass,
                mu = mu,
                angular_velocity = angularVelocity,
            };
        }

        private TelemetryTarget BuildPartTargetTelemetry(
            ModApi.Flight.UI.INavSphereTarget navTarget,
            TelemetryVector3 position,
            TelemetryVector3 velocity)
        {
            var partScript = navTarget as ModApi.Craft.Parts.IPartScript;
            var partId = partScript?.Data != null ? (int?)partScript.Data.Id : null;
            var partName = partScript?.Data?.Name ?? navTarget.Name;
            var parentCraftNode = partScript?.CraftScript?.CraftNode;
            var parentCraftId = parentCraftNode != null ? FormatCraftId(parentCraftNode.NodeId) : null;
            var parentCraftName = parentCraftNode != null ? GetCraftName(parentCraftNode) : null;

            return new TelemetryTarget
            {
                target_type = "part",
                target_id = partId.HasValue ? $"part_{partId.Value}" : navTarget.Name ?? "part_target",
                name = navTarget.Name ?? partName ?? string.Empty,
                parent_body = BuildBodyRef(navTarget.Parent),
                position = position,
                velocity = velocity,
                part_id = partId,
                part_name = partName,
                parent_craft_id = parentCraftId,
                parent_craft_name = parentCraftName,
            };
        }

        private TelemetryTarget BuildLandmarkTargetTelemetry(
            ModApi.Flight.UI.INavSphereTarget navTarget,
            TelemetryVector3 position,
            TelemetryVector3 velocity)
        {
            var targetObject = navTarget as object;
            var landmarkId = ReadStringProperty(targetObject, "Id", navTarget.Name);
            var latitudeDeg = TryReadLatitudeOrLongitudeDegrees(targetObject, "Latitude");
            var longitudeDeg = TryReadLatitudeOrLongitudeDegrees(targetObject, "Longitude");

            return new TelemetryTarget
            {
                target_type = "landmark",
                target_id = landmarkId ?? navTarget.Name ?? "landmark_target",
                name = navTarget.Name ?? string.Empty,
                parent_body = BuildBodyRef(navTarget.Parent),
                position = position,
                velocity = velocity,
                landmark_id = landmarkId,
                latitude_deg = latitudeDeg,
                longitude_deg = longitudeDeg,
            };
        }

        private TelemetryTarget BuildPositionTargetTelemetry(
            ModApi.Flight.UI.INavSphereTarget navTarget,
            TelemetryVector3 position,
            TelemetryVector3 velocity)
        {
            var targetObject = navTarget as object;
            var targetId = ReadStringProperty(targetObject, "Id", navTarget.Name);
            if (string.IsNullOrWhiteSpace(targetId))
            {
                targetId = "position_target";
            }

            return new TelemetryTarget
            {
                target_type = "position",
                target_id = targetId,
                name = navTarget.Name ?? string.Empty,
                parent_body = BuildBodyRef(navTarget.Parent),
                position = position,
                velocity = velocity,
            };
        }

        private static string ClassifyTargetType(ModApi.Flight.UI.INavSphereTarget navTarget)
        {
            if (GetCraftNodeFromTarget(navTarget) != null)
            {
                return "craft";
            }

            if (GetPlanetNodeFromTarget(navTarget) != null)
            {
                return "body";
            }

            if (navTarget is ModApi.Craft.Parts.IPartScript)
            {
                return "part";
            }

            var typeName = navTarget.GetType().FullName ?? string.Empty;
            if (typeName == PositionTargetTypeName)
            {
                return "position";
            }

            if (typeName == PartTargetTypeName)
            {
                return "part";
            }

            if (typeName.Contains("Landmark", StringComparison.Ordinal)
                || typeName.Contains("LaunchLocation", StringComparison.Ordinal))
            {
                return "landmark";
            }

            return "position";
        }

        private static ModApi.Craft.ICraftNode GetCraftNodeFromTarget(ModApi.Flight.UI.INavSphereTarget navTarget)
        {
            if (navTarget is ModApi.Craft.ICraftNode craftNode)
            {
                return craftNode;
            }

            return navTarget?.OrbitNode as ModApi.Craft.ICraftNode;
        }

        private static ModApi.Flight.Sim.IPlanetNode GetPlanetNodeFromTarget(ModApi.Flight.UI.INavSphereTarget navTarget)
        {
            if (navTarget is ModApi.Flight.Sim.IPlanetNode planetNode)
            {
                return planetNode;
            }

            return navTarget?.OrbitNode as ModApi.Flight.Sim.IPlanetNode;
        }

        private static TelemetryBodyRef BuildBodyRef(ModApi.Flight.Sim.IPlanetNode planetNode)
        {
            if (planetNode == null)
            {
                return null;
            }

            var planetData = planetNode.PlanetData;
            return new TelemetryBodyRef
            {
                body_id = FormatBodyId(planetData?.Id) ?? string.Empty,
                name = planetData?.Name ?? planetNode.Name ?? string.Empty,
            };
        }

        private static TelemetryBodyRef BuildBodyRef(ModApi.Planet.IPlanetData planetData)
        {
            if (planetData == null)
            {
                return null;
            }

            return new TelemetryBodyRef
            {
                body_id = FormatBodyId(planetData.Id) ?? string.Empty,
                name = planetData.Name ?? string.Empty,
            };
        }

        private static string FormatBodyId(object bodyId)
        {
            return bodyId?.ToString();
        }

        private static TelemetryVector3 CreateTelemetryVector3(Vector3d value)
        {
            return new TelemetryVector3
            {
                x = (float)value.x,
                y = (float)value.y,
                z = (float)value.z,
            };
        }

        private static string ReadStringProperty(object target, string propertyName, string fallback = null)
        {
            if (target == null)
            {
                return fallback;
            }

            var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                return fallback;
            }

            var value = property.GetValue(target, null);
            return value?.ToString() ?? fallback;
        }

        private static float? TryReadLatitudeOrLongitudeDegrees(object target, string propertyName)
        {
            if (target == null)
            {
                return null;
            }

            var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                return null;
            }

            var raw = property.GetValue(target, null);
            switch (raw)
            {
                case float floatValue:
                    return ConvertAngleToDegrees(floatValue);
                case double doubleValue:
                    return ConvertAngleToDegrees(doubleValue);
                default:
                    return null;
            }
        }

        private static float ConvertAngleToDegrees(double angle)
        {
            if (Math.Abs(angle) <= Math.PI + 1e-6)
            {
                return (float)(angle * (180.0 / Math.PI));
            }

            return (float)angle;
        }

        private static int CountItems(IEnumerable items)
        {
            if (items == null)
            {
                return 0;
            }

            if (items is ICollection collection)
            {
                return collection.Count;
            }

            var count = 0;
            var enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            return count;
        }

        private static Vector3d BuildLiftForceVector(ModApi.Craft.ICraftScript craftScript)
        {
            var totalLiftForce = new Vector3d(0d, 0d, 0d);
            var parts = craftScript.Data?.Assembly?.Parts;
            if (parts == null)
            {
                return totalLiftForce;
            }

            foreach (ModApi.Craft.Parts.PartData partData in parts)
            {
                var modifiers = partData?.PartScript?.Modifiers;
                if (modifiers == null)
                {
                    continue;
                }

                foreach (var modifier in modifiers)
                {
                    if (modifier == null)
                    {
                        continue;
                    }

                    var modifierType = modifier.GetType();
                    if (modifierType.FullName == WingModifierTypeName)
                    {
                        totalLiftForce += ReadVector3Property(modifier, "LiftForceVector");
                    }
                    else if (modifierType.FullName == PropellerModifierTypeName)
                    {
                        totalLiftForce += ReadVector3Field(modifier, "CalculatedLiftForce");
                    }
                }
            }

            return totalLiftForce;
        }

        private static Vector3d BuildDragForceVector(ModApi.Craft.ICraftScript craftScript)
        {
            var totalDragForce = new Vector3d(0d, 0d, 0d);
            var bodies = craftScript.Data?.Assembly?.Bodies;
            if (bodies == null)
            {
                return totalDragForce;
            }

            foreach (ModApi.Craft.BodyData bodyData in bodies)
            {
                var bodyScript = bodyData?.BodyScript;
                if (bodyScript == null)
                {
                    continue;
                }

                var bodyDragForce = bodyScript.DragForce;
                totalDragForce += new Vector3d(bodyDragForce.x, bodyDragForce.y, bodyDragForce.z);
            }

            var dragForceScale = (double)ModApi.Constants.DragForceScale;
            if (Math.Abs(dragForceScale) > double.Epsilon)
            {
                totalDragForce /= dragForceScale;
            }

            return totalDragForce;
        }

        private static Vector3d BuildEngineForceVector(ModApi.Craft.ICraftScript craftScript)
        {
            var flightData = craftScript.FlightData;
            var activeEngines = flightData.ActiveEngines;
            if (activeEngines == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            var weightedDirection = new Vector3d(0d, 0d, 0d);
            foreach (ModApi.Craft.Parts.Modifiers.Propulsion.IReactionEngine engine in activeEngines)
            {
                if (engine == null || !engine.IsActive || engine.CurrentThrust <= float.Epsilon)
                {
                    continue;
                }

                var direction = GetEngineForceDirection(engine.Part?.PartScript);
                if (MagnitudeSquared(direction) <= 1e-12)
                {
                    continue;
                }

                weightedDirection += direction * engine.CurrentThrust;
            }

            var totalThrust = (double)flightData.CurrentEngineThrustUnscaled;
            if (totalThrust <= double.Epsilon)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            if (MagnitudeSquared(weightedDirection) <= 1e-12)
            {
                return NormalizeOrZero(flightData.CraftForward) * totalThrust;
            }

            return NormalizeOrZero(weightedDirection) * totalThrust;
        }

        private static Vector3d GetEngineForceDirection(ModApi.Craft.Parts.IPartScript part)
        {
            if (part == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            var modifiers = part.Modifiers;
            if (modifiers != null)
            {
                foreach (var modifier in modifiers)
                {
                    if (modifier == null || modifier.GetType().FullName != "Assets.Scripts.Craft.Parts.Modifiers.Propulsion.EngineNozzleScript")
                    {
                        continue;
                    }

                    var forceTransform = ForceNozzleTransformField?.GetValue(modifier) as Transform;
                    if (forceTransform != null)
                    {
                        return NormalizeOrZero(new Vector3d(
                            forceTransform.forward.x,
                            forceTransform.forward.y,
                            forceTransform.forward.z));
                    }
                }
            }

            var partTransform = part.Transform;
            if (partTransform != null)
            {
                return NormalizeOrZero(new Vector3d(
                    partTransform.forward.x,
                    partTransform.forward.y,
                    partTransform.forward.z));
            }

            return new Vector3d(0d, 0d, 0d);
        }

        private static Vector3d NormalizeOrZero(Vector3d vector)
        {
            var magnitudeSquared = MagnitudeSquared(vector);
            if (magnitudeSquared <= 1e-12)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            return vector / Math.Sqrt(magnitudeSquared);
        }

        private static double MagnitudeSquared(Vector3d vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        private static Vector3d BuildEastFromLatLon(Vector2d latLon)
        {
            var longitude = latLon.y;
            var east = new Vector3d(-Math.Sin(longitude), 0d, Math.Cos(longitude));
            return NormalizeOrZero(east);
        }

        private static Vector3d ReadVector3Property(object target, string propertyName)
        {
            if (target == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            return ToVector3d(property.GetValue(target, null));
        }

        private static float ReadFloatProperty(object target, string propertyName, float fallback = 0f)
        {
            if (target == null)
            {
                return fallback;
            }

            var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                return fallback;
            }

            var value = property.GetValue(target, null);
            if (value == null)
            {
                return fallback;
            }

            try
            {
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return fallback;
            }
        }

        private static Vector3d ReadVector3Field(object target, string fieldName)
        {
            if (target == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            return ToVector3d(field.GetValue(target));
        }

        private static Vector3d ToVector3d(object value)
        {
            if (value == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            if (value is Vector3 vector3)
            {
                return new Vector3d(vector3.x, vector3.y, vector3.z);
            }

            if (value is Vector3d vector3d)
            {
                return vector3d;
            }

            var valueType = value.GetType();
            var xProperty = valueType.GetProperty("x") ?? valueType.GetProperty("X");
            var yProperty = valueType.GetProperty("y") ?? valueType.GetProperty("Y");
            var zProperty = valueType.GetProperty("z") ?? valueType.GetProperty("Z");

            if (xProperty == null || yProperty == null || zProperty == null)
            {
                return new Vector3d(0d, 0d, 0d);
            }

            return new Vector3d(
                Convert.ToDouble(xProperty.GetValue(value, null), CultureInfo.InvariantCulture),
                Convert.ToDouble(yProperty.GetValue(value, null), CultureInfo.InvariantCulture),
                Convert.ToDouble(zProperty.GetValue(value, null), CultureInfo.InvariantCulture));
        }

        private static float GetFrameDeltaTime(ModApi.Flight.IFlightScene flightScene)
        {
            var timeManager = flightScene?.TimeManager;
            if (timeManager != null)
            {
                return (float)timeManager.DeltaTime;
            }

            return Time.deltaTime;
        }

        private static float GetWarpAmount(ModApi.Flight.IFlightScene flightScene)
        {
            var currentMode = flightScene?.TimeManager?.CurrentMode;
            if (currentMode != null)
            {
                return (float)currentMode.TimeMultiplier;
            }

            return 1f;
        }

        private static float GetTotalTimeSeconds(ModApi.Flight.IFlightScene flightScene)
        {
            var flightState = flightScene?.FlightState;
            var totalFlightTime = ReadFloatProperty(flightState, "TotalFlightTimeInRealtimeSeconds", -1f);
            if (totalFlightTime >= 0f)
            {
                return totalFlightTime;
            }

            return Time.time;
        }

        private static float GetRealTimeSeconds(ModApi.Craft.ICraftScript craftScript)
        {
            var flightData = craftScript?.FlightData;
            var realTime = ReadFloatProperty(flightData, "TimeReal", -1f);
            if (realTime >= 0f)
            {
                return realTime;
            }

            return Time.time;
        }

        private static float GetTimeSinceLaunchSeconds(ModApi.Craft.ICraftScript craftScript, float fallback)
        {
            var parts = craftScript?.Data?.Assembly?.Parts;
            if (parts == null)
            {
                return fallback;
            }

            foreach (ModApi.Craft.Parts.PartData partData in parts)
            {
                var modifiers = partData?.PartScript?.Modifiers;
                if (modifiers == null)
                {
                    continue;
                }

                foreach (var modifier in modifiers)
                {
                    if (modifier == null || modifier.GetType().FullName != FlightProgramDataTypeName)
                    {
                        continue;
                    }

                    return ReadFloatProperty(modifier, "TimeSinceLaunch", fallback);
                }
            }

            return fallback;
        }

        private static bool TryParseDisplayMessage(string json, out string message, out float duration)
        {
            message = null;
            duration = 0f;

            if (!TryReadJsonObject(json, "\"display\"", out var displayJson))
            {
                return false;
            }

            if (!TryReadJsonString(displayJson, "\"message\"", out message))
            {
                return false;
            }

            if (!TryReadJsonFloat(displayJson, "\"duration\"", out duration))
            {
                duration = 2f;
            }

            duration = Mathf.Max(0f, duration);
            return !string.IsNullOrEmpty(message);
        }

        private static bool TryParseFlightLogMessage(string json, out string message, out bool replace)
        {
            message = null;
            replace = false;

            if (!TryReadJsonObject(json, "\"flight_log\"", out var flightLogJson))
            {
                return false;
            }

            if (!TryReadJsonString(flightLogJson, "\"message\"", out message))
            {
                return false;
            }

            TryReadJsonBool(flightLogJson, "\"replace\"", out replace);
            return !string.IsNullOrEmpty(message);
        }

        private static bool TryParseStageActivation(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("\"stage\""))
            {
                return false;
            }

            return TryReadJsonBool(json, "\"activate_next\"", out var activateNext) && activateNext;
        }

        private static bool TryParseRpcRequest(string json, out int requestId, out string method)
        {
            requestId = 0;
            method = null;

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            if (!TryReadJsonString(json, "\"type\"", out var messageType)
                || !string.Equals(messageType, "rpc_request", StringComparison.Ordinal))
            {
                return false;
            }

            if (!TryReadJsonInt(json, "\"id\"", out requestId))
            {
                return false;
            }

            return TryReadJsonString(json, "\"method\"", out method) && !string.IsNullOrEmpty(method);
        }

        private static bool TryParseTrackedCraftIds(string json, out List<string> craftIds)
        {
            craftIds = new List<string>();

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            return TryReadJsonStringArray(json, "\"craft_ids\"", craftIds);
        }

        private static bool TryParseBodyRequest(string json, out string bodyKey)
        {
            bodyKey = null;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson))
            {
                return false;
            }

            TryReadJsonString(argsJson, "\"body\"", out bodyKey);
            return true;
        }

        private static bool TryParseWorldPositionRequest(string json, out Vector3d position, out string bodyKey)
        {
            position = Vector3d.zero;
            bodyKey = null;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonObject(argsJson, "\"position\"", out var positionJson)
                || !TryReadJsonFloat(positionJson, "\"x\"", out var x)
                || !TryReadJsonFloat(positionJson, "\"y\"", out var y)
                || !TryReadJsonFloat(positionJson, "\"z\"", out var z))
            {
                return false;
            }

            position = new Vector3d(x, y, z);
            TryReadJsonString(argsJson, "\"body\"", out bodyKey);
            return true;
        }

        private static bool TryParseWorldLatLonRequest(string json, out double latDeg, out double lonDeg, out string bodyKey)
        {
            latDeg = 0d;
            lonDeg = 0d;
            bodyKey = null;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonFloat(argsJson, "\"lat_deg\"", out var lat)
                || !TryReadJsonFloat(argsJson, "\"lon_deg\"", out var lon))
            {
                return false;
            }

            latDeg = lat;
            lonDeg = lon;
            TryReadJsonString(argsJson, "\"body\"", out bodyKey);
            return true;
        }

        private static bool TryParseWorldSurfaceRequest(
            string json,
            out double latDeg,
            out double lonDeg,
            out double altitude,
            out string bodyKey,
            out string reference)
        {
            latDeg = 0d;
            lonDeg = 0d;
            altitude = 0d;
            bodyKey = null;
            reference = "agl";
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonFloat(argsJson, "\"lat_deg\"", out var lat)
                || !TryReadJsonFloat(argsJson, "\"lon_deg\"", out var lon)
                || !TryReadJsonFloat(argsJson, "\"altitude\"", out var alt))
            {
                return false;
            }

            latDeg = lat;
            lonDeg = lon;
            altitude = alt;
            TryReadJsonString(argsJson, "\"body\"", out bodyKey);
            if (TryReadJsonString(argsJson, "\"reference\"", out var parsedReference) && !string.IsNullOrEmpty(parsedReference))
            {
                reference = parsedReference;
            }

            return true;
        }

        private static bool TryParseWorldRayRequest(
            string json,
            out Vector3d origin,
            out Vector3d direction,
            out float maxDistance)
        {
            origin = Vector3d.zero;
            direction = Vector3d.zero;
            maxDistance = ModApi.Constants.MaxRaycastDistance;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonObject(argsJson, "\"origin\"", out var originJson)
                || !TryReadJsonObject(argsJson, "\"direction\"", out var directionJson)
                || !TryReadJsonFloat(originJson, "\"x\"", out var ox)
                || !TryReadJsonFloat(originJson, "\"y\"", out var oy)
                || !TryReadJsonFloat(originJson, "\"z\"", out var oz)
                || !TryReadJsonFloat(directionJson, "\"x\"", out var dx)
                || !TryReadJsonFloat(directionJson, "\"y\"", out var dy)
                || !TryReadJsonFloat(directionJson, "\"z\"", out var dz))
            {
                return false;
            }

            origin = new Vector3d(ox, oy, oz);
            direction = new Vector3d(dx, dy, dz);
            if (TryReadJsonFloat(argsJson, "\"max_distance\"", out var parsedMaxDistance) && parsedMaxDistance > 0f)
            {
                maxDistance = parsedMaxDistance;
            }

            return direction.sqrMagnitude > double.Epsilon;
        }

        private static bool TryParseOptionalCraftRequest(string json, out string craftId)
        {
            craftId = null;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson))
            {
                return false;
            }

            TryReadJsonString(argsJson, "\"craft_id\"", out craftId);
            return true;
        }

        private static bool TryParsePartLookupRequest(string json, out int partId, out string craftId)
        {
            partId = 0;
            craftId = null;
            if (!TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonInt(argsJson, "\"part_id\"", out partId))
            {
                return false;
            }

            TryReadJsonString(argsJson, "\"craft_id\"", out craftId);
            return true;
        }

        private static bool TryParsePartVectorRequest(string json, out int partId, out string craftId, out Vector3d vector)
        {
            vector = Vector3d.zero;
            if (!TryParsePartLookupRequest(json, out partId, out craftId)
                || !TryReadJsonObject(json, "\"args\"", out var argsJson)
                || !TryReadJsonObject(argsJson, "\"vector\"", out var vectorJson)
                || !TryReadJsonFloat(vectorJson, "\"x\"", out var x)
                || !TryReadJsonFloat(vectorJson, "\"y\"", out var y)
                || !TryReadJsonFloat(vectorJson, "\"z\"", out var z))
            {
                return false;
            }

            vector = new Vector3d(x, y, z);
            return true;
        }

        private static bool TryParseControlMessage(
            string json,
            out int sequence,
            out bool hasThrottle,
            out float throttle,
            out bool hasPitch,
            out float pitch,
            out bool hasRoll,
            out float roll,
            out bool hasYaw,
            out float yaw,
            out bool hasBrake,
            out float brake,
            out bool hasTranslateForward,
            out float translateForward,
            out bool hasTranslateRight,
            out float translateRight,
            out bool hasTranslateUp,
            out float translateUp,
            out bool hasSlider1,
            out float slider1,
            out bool hasSlider2,
            out float slider2,
            out bool hasSlider3,
            out float slider3,
            out bool hasSlider4,
            out float slider4,
            out bool hasTranslationModeEnabled,
            out bool translationModeEnabled)
        {
            sequence = 0;
            hasThrottle = false;
            throttle = 0f;
            hasPitch = false;
            pitch = 0f;
            hasRoll = false;
            roll = 0f;
            hasYaw = false;
            yaw = 0f;
            hasBrake = false;
            brake = 0f;
            hasTranslateForward = false;
            translateForward = 0f;
            hasTranslateRight = false;
            translateRight = 0f;
            hasTranslateUp = false;
            translateUp = 0f;
            hasSlider1 = false;
            slider1 = 0f;
            hasSlider2 = false;
            slider2 = 0f;
            hasSlider3 = false;
            slider3 = 0f;
            hasSlider4 = false;
            slider4 = 0f;
            hasTranslationModeEnabled = false;
            translationModeEnabled = false;

            if (!TryReadJsonObject(json, "\"control\"", out var controlJson))
            {
                return false;
            }

            TryReadJsonInt(json, "\"seq\"", out sequence);
            hasThrottle = TryReadJsonFloat(controlJson, "\"throttle\"", out throttle);
            hasPitch = TryReadJsonFloat(controlJson, "\"pitch\"", out pitch);
            hasRoll = TryReadJsonFloat(controlJson, "\"roll\"", out roll);
            hasYaw = TryReadJsonFloat(controlJson, "\"yaw\"", out yaw);
            hasBrake = TryReadJsonFloat(controlJson, "\"brake\"", out brake);
            hasTranslateForward = TryReadJsonFloat(controlJson, "\"translate_forward\"", out translateForward);
            hasTranslateRight = TryReadJsonFloat(controlJson, "\"translate_right\"", out translateRight);
            hasTranslateUp = TryReadJsonFloat(controlJson, "\"translate_up\"", out translateUp);
            hasSlider1 = TryReadJsonFloat(controlJson, "\"slider_1\"", out slider1);
            hasSlider2 = TryReadJsonFloat(controlJson, "\"slider_2\"", out slider2);
            hasSlider3 = TryReadJsonFloat(controlJson, "\"slider_3\"", out slider3);
            hasSlider4 = TryReadJsonFloat(controlJson, "\"slider_4\"", out slider4);
            hasTranslationModeEnabled = TryReadJsonBool(controlJson, "\"translation_mode_enabled\"", out translationModeEnabled);
            return true;
        }

        private static bool TryParseTimeWarpMode(string json, out int mode)
        {
            mode = 0;
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("\"time_warp\""))
            {
                return false;
            }

            return TryReadJsonInt(json, "\"mode\"", out mode);
        }

        private static bool TryParseHeadingLockMode(string json, out string mode)
        {
            mode = null;
            if (!TryReadJsonObject(json, "\"heading_lock\"", out var headingLockJson))
            {
                return false;
            }

            return TryReadJsonString(headingLockJson, "\"mode\"", out mode) && !string.IsNullOrEmpty(mode);
        }

        private static bool TryParseHeadingLockVector(string json, out Vector3d vector)
        {
            vector = new Vector3d(0d, 0d, 0d);
            if (!TryReadJsonObject(json, "\"heading_lock_vector\"", out var vectorJson))
            {
                return false;
            }

            return TryReadJsonFloat(vectorJson, "\"x\"", out var x)
                && TryReadJsonFloat(vectorJson, "\"y\"", out var y)
                && TryReadJsonFloat(vectorJson, "\"z\"", out var z)
                && AssignVector(x, y, z, out vector);
        }

        private static bool TryParseAttitudeTarget(string json, out float? headingDeg, out float? pitchDeg)
        {
            headingDeg = null;
            pitchDeg = null;
            if (!TryReadJsonObject(json, "\"attitude_target\"", out var attitudeJson))
            {
                return false;
            }

            if (TryReadJsonFloat(attitudeJson, "\"heading_deg\"", out var heading))
            {
                headingDeg = heading;
            }

            if (TryReadJsonFloat(attitudeJson, "\"pitch_deg\"", out var pitch))
            {
                pitchDeg = pitch;
            }

            return headingDeg.HasValue || pitchDeg.HasValue;
        }

        private static bool TryParseActivationGroupCommand(string json, out int group, out bool enabled)
        {
            group = 0;
            enabled = false;
            if (!TryReadJsonObject(json, "\"activation_group\"", out var activationGroupJson))
            {
                return false;
            }

            return TryReadJsonInt(activationGroupJson, "\"group\"", out group)
                && TryReadJsonBool(activationGroupJson, "\"enabled\"", out enabled);
        }

        private static bool TryParseTargetNodeCommand(string json, out string name)
        {
            name = null;
            if (!TryReadJsonObject(json, "\"target_node\"", out var targetNodeJson))
            {
                return false;
            }

            return TryReadJsonString(targetNodeJson, "\"name\"", out name) && !string.IsNullOrEmpty(name);
        }

        private static bool TryParsePidGainsCommand(string json, string key, out Vector3 pid)
        {
            pid = Vector3.zero;
            if (!TryReadJsonObject(json, key, out var pidJson))
            {
                return false;
            }

            return TryReadJsonFloat(pidJson, "\"p\"", out var p)
                && TryReadJsonFloat(pidJson, "\"i\"", out var i)
                && TryReadJsonFloat(pidJson, "\"d\"", out var d)
                && AssignVector3(p, i, d, out pid);
        }

        private static bool TryParseMessageCommands(string json, out List<PendingMessageCommand> commands)
        {
            commands = new List<PendingMessageCommand>();
            if (!TryReadJsonArray(json, "\"messages\"", out var messagesJson))
            {
                return false;
            }

            if (!TryReadJsonObjectArray(messagesJson, out var messageJsonObjects))
            {
                return false;
            }

            foreach (var messageJson in messageJsonObjects)
            {
                if (!TryReadJsonString(messageJson, "\"scope\"", out var scope)
                    || string.IsNullOrWhiteSpace(scope)
                    || !TryReadJsonString(messageJson, "\"message\"", out var message)
                    || string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                var command = new PendingMessageCommand
                {
                    scope = scope,
                    message = message,
                    data_json = "null",
                };

                TryReadJsonString(messageJson, "\"craft_id\"", out command.craft_id);
                if (TryReadJsonFloat(messageJson, "\"radius\"", out var radius) && radius > 0f)
                {
                    command.radius = radius;
                }

                if (TryReadJsonRawValue(messageJson, "\"data\"", out var rawDataJson))
                {
                    command.data_json = rawDataJson;
                }

                commands.Add(command);
            }

            return commands.Count > 0;
        }

        private static bool TryReadJsonInt(string json, string key, out int value)
        {
            value = 0;
            return TryReadJsonToken(json, key, out var token)
                && int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryReadJsonFloat(string json, string key, out float value)
        {
            value = 0f;
            return TryReadJsonToken(json, key, out var token)
                && float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryReadJsonBool(string json, string key, out bool value)
        {
            value = false;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var startIndex = colonIndex + 1;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
            {
                startIndex++;
            }

            if (startIndex + 4 <= json.Length
                && string.Compare(json, startIndex, "true", 0, 4, StringComparison.Ordinal) == 0)
            {
                value = true;
                return true;
            }

            if (startIndex + 5 <= json.Length
                && string.Compare(json, startIndex, "false", 0, 5, StringComparison.Ordinal) == 0)
            {
                value = false;
                return true;
            }

            return false;
        }

        private static bool TryReadJsonStringArray(string json, string key, List<string> values)
        {
            values.Clear();

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var index = colonIndex + 1;
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            if (index >= json.Length || json[index] != '[')
            {
                return false;
            }

            index++;
            while (index < json.Length)
            {
                while (index < json.Length && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }

                if (index < json.Length && json[index] == ']')
                {
                    return true;
                }

                if (!TryReadJsonStringAt(json, ref index, out var value))
                {
                    return false;
                }

                values.Add(value);

                while (index < json.Length && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }

                if (index < json.Length && json[index] == ',')
                {
                    index++;
                    continue;
                }

                if (index < json.Length && json[index] == ']')
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool TryReadJsonArray(string json, string key, out string value)
        {
            value = null;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var startIndex = colonIndex + 1;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
            {
                startIndex++;
            }

            if (startIndex >= json.Length || json[startIndex] != '[')
            {
                return false;
            }

            if (!TryReadJsonComposite(json, startIndex, '[', ']', out value))
            {
                return false;
            }

            return true;
        }

        private static bool TryReadJsonObject(string json, string key, out string value)
        {
            value = null;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var startIndex = colonIndex + 1;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
            {
                startIndex++;
            }

            if (startIndex >= json.Length || json[startIndex] != '{')
            {
                return false;
            }

            return TryReadJsonComposite(json, startIndex, '{', '}', out value);
        }

        private static bool AssignVector(float x, float y, float z, out Vector3d vector)
        {
            vector = new Vector3d(x, y, z);
            return true;
        }

        private static bool AssignVector3(float x, float y, float z, out Vector3 vector)
        {
            vector = new Vector3(x, y, z);
            return true;
        }

        private static bool TryReadJsonToken(string json, string key, out string token)
        {
            token = null;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var startIndex = colonIndex + 1;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
            {
                startIndex++;
            }

            var endIndex = startIndex;
            while (endIndex < json.Length)
            {
                var character = json[endIndex];
                if ((character >= '0' && character <= '9')
                    || character == '-'
                    || character == '+'
                    || character == '.'
                    || character == 'e'
                    || character == 'E')
                {
                    endIndex++;
                    continue;
                }

                break;
            }

            if (endIndex <= startIndex)
            {
                return false;
            }

            token = json.Substring(startIndex, endIndex - startIndex);
            return true;
        }

        private static bool TryReadJsonString(string json, string key, out string value)
        {
            value = null;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var startIndex = colonIndex + 1;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
            {
                startIndex++;
            }

            if (startIndex >= json.Length || json[startIndex] != '"')
            {
                return false;
            }

            var builder = new StringBuilder();
            for (var i = startIndex + 1; i < json.Length; i++)
            {
                var character = json[i];
                if (character == '\\')
                {
                    if (i + 1 >= json.Length)
                    {
                        return false;
                    }

                    var escaped = json[++i];
                    switch (escaped)
                    {
                        case '"':
                            builder.Append('"');
                            break;
                        case '\\':
                            builder.Append('\\');
                            break;
                        case '/':
                            builder.Append('/');
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            if (i + 4 >= json.Length)
                            {
                                return false;
                            }

                            var hex = json.Substring(i + 1, 4);
                            if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
                            {
                                return false;
                            }

                            builder.Append((char)codePoint);
                            i += 4;
                            break;
                        default:
                            return false;
                    }

                    continue;
                }

                if (character == '"')
                {
                    value = builder.ToString();
                    return true;
                }

                builder.Append(character);
            }

            return false;
        }

        private static bool TryReadJsonRawValue(string json, string key, out string value)
        {
            value = null;

            var keyIndex = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return false;
            }

            var colonIndex = json.IndexOf(':', keyIndex + key.Length);
            if (colonIndex < 0)
            {
                return false;
            }

            var index = colonIndex + 1;
            return TryReadJsonValueAt(json, ref index, out value);
        }

        private static bool TryReadJsonObjectArray(string json, out List<string> objects)
        {
            objects = new List<string>();
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            var index = 0;
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            if (index >= json.Length || json[index] != '[')
            {
                return false;
            }

            index++;
            while (index < json.Length)
            {
                while (index < json.Length && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }

                if (index < json.Length && json[index] == ']')
                {
                    return true;
                }

                if (!TryReadJsonValueAt(json, ref index, out var value) || string.IsNullOrWhiteSpace(value) || value[0] != '{')
                {
                    return false;
                }

                objects.Add(value);

                while (index < json.Length && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }

                if (index < json.Length && json[index] == ',')
                {
                    index++;
                    continue;
                }

                if (index < json.Length && json[index] == ']')
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool TryReadJsonComposite(string json, int startIndex, char open, char close, out string value)
        {
            value = null;
            var depth = 0;
            var inString = false;
            for (var i = startIndex; i < json.Length; i++)
            {
                var character = json[i];
                if (character == '"' && (i == 0 || json[i - 1] != '\\'))
                {
                    inString = !inString;
                }

                if (inString)
                {
                    continue;
                }

                if (character == open)
                {
                    depth++;
                }
                else if (character == close)
                {
                    depth--;
                    if (depth == 0)
                    {
                        value = json.Substring(startIndex, i - startIndex + 1);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryReadJsonValueAt(string json, ref int index, out string value)
        {
            value = null;

            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            if (index >= json.Length)
            {
                return false;
            }

            var startIndex = index;
            var character = json[index];
            if (character == '"')
            {
                if (!TryReadJsonStringAt(json, ref index, out _))
                {
                    return false;
                }

                value = json.Substring(startIndex, index - startIndex);
                return true;
            }

            if (character == '{')
            {
                if (!TryReadJsonComposite(json, index, '{', '}', out value))
                {
                    return false;
                }

                index += value.Length;
                return true;
            }

            if (character == '[')
            {
                if (!TryReadJsonComposite(json, index, '[', ']', out value))
                {
                    return false;
                }

                index += value.Length;
                return true;
            }

            while (index < json.Length)
            {
                character = json[index];
                if (character == ',' || character == '}' || character == ']')
                {
                    break;
                }

                index++;
            }

            value = json.Substring(startIndex, index - startIndex).Trim();
            return !string.IsNullOrEmpty(value);
        }

        private static bool TryReadJsonStringAt(string json, ref int index, out string value)
        {
            value = null;

            if (index >= json.Length || json[index] != '"')
            {
                return false;
            }

            var builder = new StringBuilder();
            for (var i = index + 1; i < json.Length; i++)
            {
                var character = json[i];
                if (character == '\\')
                {
                    if (i + 1 >= json.Length)
                    {
                        return false;
                    }

                    var escaped = json[++i];
                    switch (escaped)
                    {
                        case '"':
                            builder.Append('"');
                            break;
                        case '\\':
                            builder.Append('\\');
                            break;
                        case '/':
                            builder.Append('/');
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            if (i + 4 >= json.Length)
                            {
                                return false;
                            }

                            var hex = json.Substring(i + 1, 4);
                            if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
                            {
                                return false;
                            }

                            builder.Append((char)codePoint);
                            i += 4;
                            break;
                        default:
                            return false;
                    }

                    continue;
                }

                if (character == '"')
                {
                    value = builder.ToString();
                    index = i + 1;
                    return true;
                }

                builder.Append(character);
            }

            return false;
        }

        private static void ShowFlightMessage(string message, float duration)
        {
            var flightSceneUi = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI;
            if (flightSceneUi == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            flightSceneUi.ShowMessage(message, false, duration);
        }

        private void ShowFlightLogMessage(string message, bool replace)
        {
            var flightLog = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI?.FlightLog;
            if (flightLog == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            if (replace)
            {
                if (_dynamicFlightLogEntryId >= 0)
                {
                    try
                    {
                        flightLog.UpdateLogEntry(_dynamicFlightLogEntryId, message);
                        return;
                    }
                    catch
                    {
                        _dynamicFlightLogEntryId = -1;
                    }
                }

                var entry = flightLog.AddLog(
                    message,
                    ModApi.Flight.UI.FlightLogEntryCategory.Vizzy,
                    true,
                    null);
                if (entry != null)
                {
                    _dynamicFlightLogEntryId = entry.Id;
                }

                return;
            }

            flightLog.AddLog(
                message,
                ModApi.Flight.UI.FlightLogEntryCategory.Vizzy,
                false,
                null);
        }

        private static void ActivateNextStage()
        {
            var activeCommandPod = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.ActiveCommandPod;
            if (activeCommandPod == null)
            {
                return;
            }

            activeCommandPod.ActivateStage();
        }

        private static void SetTimeWarpMode(int mode)
        {
            var timeManager = ModApi.Common.Game.Instance.FlightScene?.TimeManager;
            if (timeManager == null)
            {
                return;
            }

            if (!timeManager.CanSetTimeMultiplierMode(mode, out _))
            {
                return;
            }

            timeManager.SetMode(mode, false);
        }

        private static void ApplyHeadingLockMode(string mode)
        {
            var navSphere = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI?.NavSphere;
            if (navSphere == null || string.IsNullOrWhiteSpace(mode))
            {
                return;
            }

            switch (mode.Trim().ToLowerInvariant())
            {
                case "none":
                    navSphere.UnlockHeading();
                    return;
                case "prograde":
                    navSphere.UnlockHeading();
                    navSphere.ToggleLock(ModApi.Flight.UI.NavSphereIndicatorType.VelocityPrograde);
                    return;
                case "retrograde":
                    navSphere.UnlockHeading();
                    navSphere.ToggleLock(ModApi.Flight.UI.NavSphereIndicatorType.VelocityRetrograde);
                    return;
                case "target":
                    navSphere.UnlockHeading();
                    navSphere.ToggleLock(ModApi.Flight.UI.NavSphereIndicatorType.Target);
                    return;
                case "burnnode":
                    navSphere.UnlockHeading();
                    navSphere.ToggleLock(ModApi.Flight.UI.NavSphereIndicatorType.ManeuverNode);
                    return;
                case "current":
                    navSphere.LockCurrentHeading();
                    return;
            }
        }

        private static void LockHeadingVector(Vector3d vector)
        {
            var navSphere = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI?.NavSphere;
            if (navSphere == null)
            {
                return;
            }

            navSphere.LockHeading(vector);
        }

        private static void ApplyAttitudeTarget(float? headingDeg, float? pitchDeg)
        {
            var navSphere = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI?.NavSphere;
            if (navSphere == null)
            {
                return;
            }

            if (headingDeg.HasValue && pitchDeg.HasValue)
            {
                navSphere.LockHeading(pitchDeg.Value, headingDeg.Value, null);
                return;
            }

            if (headingDeg.HasValue)
            {
                var currentPitch = (float)navSphere.Pitch;
                navSphere.LockHeading(currentPitch, headingDeg.Value, null);
            }
        }

        private static void SetActivationGroupState(int group, bool enabled)
        {
            var commandPod = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.ActiveCommandPod;
            if (commandPod == null || group < 1)
            {
                return;
            }

            commandPod.SetActivationGroupState(group, enabled);
        }

        private static void SetTargetNode(string name)
        {
            var navSphere = ModApi.Common.Game.Instance.FlightScene?.FlightSceneUI?.NavSphere;
            if (navSphere == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (string.Equals(name, "burnnode", StringComparison.OrdinalIgnoreCase))
            {
                navSphere.ToggleLock(ModApi.Flight.UI.NavSphereIndicatorType.ManeuverNode);
            }
        }

        private static void SetPitchPidGains(Vector3 pid)
        {
            var autopilot = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.ActiveCommandPod?.AutoPilot;
            if (autopilot == null)
            {
                return;
            }

            autopilot.PidGainsPitch = pid;
        }

        private static void SetRollPidGains(Vector3 pid)
        {
            var autopilot = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.ActiveCommandPod?.AutoPilot;
            if (autopilot == null)
            {
                return;
            }

            autopilot.PidGainsRoll = pid;
        }

        private string HandleRpcRequest(int requestId, string method, string json)
        {
            switch (method)
            {
                case "list_crafts":
                case "list_craft_names":
                    return SerializeListCraftsResponse(requestId);
                case "set_tracked_crafts":
                    if (!TryParseTrackedCraftIds(json, out var craftIds))
                    {
                        return SerializeRpcErrorResponse(
                            requestId,
                            "invalid_args",
                            "Invalid set_tracked_crafts arguments.");
                    }

                    return SerializeSetTrackedCraftsResponse(requestId, ApplyTrackedCraftIds(craftIds));
                case "list_bodies":
                    return SerializeListBodiesResponse(requestId);
                case "get_body":
                    if (!TryParseBodyRequest(json, out var bodyKey))
                    {
                        return SerializeRpcErrorResponse(
                            requestId,
                            "invalid_args",
                            "Invalid get_body arguments.");
                    }

                    return SerializeGetBodyResponse(requestId, ResolvePlanetNode(bodyKey));
                case "world_to_lat_lon_agl":
                    return SerializeWorldLatLonResponse(requestId, json, useAgl: true);
                case "world_to_lat_lon_asl":
                    return SerializeWorldLatLonResponse(requestId, json, useAgl: false);
                case "world_to_position":
                    return SerializeWorldPositionResponse(requestId, json);
                case "world_get_terrain_height":
                    return SerializeWorldTerrainHeightResponse(requestId, json);
                case "world_cast_ray":
                    return SerializeWorldCastRayResponse(requestId, json);
                case "list_parts":
                    return SerializeListPartsResponse(requestId, json);
                case "get_part":
                    return SerializeGetPartResponse(requestId, json);
                case "part_local_to_pci":
                    return SerializePartTransformResponse(requestId, json, localToPci: true);
                case "part_pci_to_local":
                    return SerializePartTransformResponse(requestId, json, localToPci: false);
                case "get_activation_group":
                    return SerializeGetActivationGroupResponse(requestId, json);
                case "set_part_activated":
                    return SerializeSetPartActivatedResponse(requestId, json);
                default:
                    return SerializeRpcErrorResponse(
                        requestId,
                        "unknown_method",
                        $"Unknown RPC method: {method}");
            }
        }

        private void SendRpcResponse(string json)
        {
            if (_rpcResponseClient == null || string.IsNullOrEmpty(json))
            {
                return;
            }

            var payload = Encoding.UTF8.GetBytes(json);
            _rpcResponseClient.Send(payload, payload.Length);
        }

        private static string SerializeListCraftsResponse(int requestId)
        {
            var builder = new StringBuilder(1024);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"crafts\":[");

            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            var first = true;
            if (craftNodes != null)
            {
                foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
                {
                    if (craftNode == null)
                    {
                        continue;
                    }

                    if (!first)
                    {
                        builder.Append(",");
                    }

                    builder.Append("{\"craft_id\":");
                    AppendJsonValue(builder, FormatCraftId(craftNode.NodeId));
                    builder.Append(",\"name\":");
                    AppendJsonValue(builder, GetCraftName(craftNode));
                    builder.Append(",\"is_active\":");
                    AppendJsonValue(builder, craftNode.IsPlayer);
                    builder.Append("}");
                    first = false;
                }
            }

            builder.Append("]}}");
            return builder.ToString();
        }

        private static string SerializeSetTrackedCraftsResponse(int requestId, IReadOnlyList<string> acceptedCraftIds)
        {
            var builder = new StringBuilder(256);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"craft_ids\":[");

            for (var i = 0; i < acceptedCraftIds.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                AppendJsonValue(builder, acceptedCraftIds[i]);
            }

            builder.Append("]}}");
            return builder.ToString();
        }

        private static string SerializeListBodiesResponse(int requestId)
        {
            var builder = new StringBuilder(2048);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"bodies\":[");

            var first = true;
            foreach (var planetNode in EnumeratePlanetNodes())
            {
                if (planetNode == null)
                {
                    continue;
                }

                if (!first)
                {
                    builder.Append(",");
                }

                AppendBodyJson(builder, planetNode);
                first = false;
            }

            builder.Append("]}}");
            return builder.ToString();
        }

        private static string SerializeGetBodyResponse(int requestId, ModApi.Flight.Sim.IPlanetNode planetNode)
        {
            var builder = new StringBuilder(1024);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"body\":");
            if (planetNode == null)
            {
                builder.Append("null");
            }
            else
            {
                AppendBodyJson(builder, planetNode);
            }

            builder.Append("}}");
            return builder.ToString();
        }

        private static string SerializeWorldLatLonResponse(int requestId, string json, bool useAgl)
        {
            if (!TryParseWorldPositionRequest(json, out var position, out var bodyKey))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid world position arguments.");
            }

            var planetNode = ResolvePlanetNode(bodyKey);
            if (planetNode == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Planet body not found.");
            }

            planetNode.GetSurfaceCoordinates(position.normalized, out var latitudeRad, out var longitudeRad);
            var altitude = useAgl
                ? ComputeAltitudeAboveGroundLevel(planetNode, position)
                : ComputeAltitudeAboveSeaLevel(planetNode, position);

            return SerializeVectorRpcResponse(
                requestId,
                latitudeRad * (180.0 / Math.PI),
                longitudeRad * (180.0 / Math.PI),
                altitude);
        }

        private static string SerializeWorldPositionResponse(int requestId, string json)
        {
            if (!TryParseWorldSurfaceRequest(
                    json,
                    out var latDeg,
                    out var lonDeg,
                    out var altitude,
                    out var bodyKey,
                    out var reference))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid world surface arguments.");
            }

            var planetNode = ResolvePlanetNode(bodyKey);
            if (planetNode == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Planet body not found.");
            }

            var altitudeType = string.Equals(reference, "asl", StringComparison.OrdinalIgnoreCase)
                ? ModApi.Flight.Sim.AltitudeType.AboveSeaLevel
                : ModApi.Flight.Sim.AltitudeType.AboveGroundLevel;
            var position = planetNode.GetSurfacePosition(
                latDeg * (Math.PI / 180.0),
                lonDeg * (Math.PI / 180.0),
                altitudeType,
                altitude,
                null);

            return SerializeVectorRpcResponse(requestId, position.x, position.y, position.z);
        }

        private static string SerializeWorldTerrainHeightResponse(int requestId, string json)
        {
            if (!TryParseWorldLatLonRequest(json, out var latDeg, out var lonDeg, out var bodyKey))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid terrain-height arguments.");
            }

            var planetNode = ResolvePlanetNode(bodyKey);
            if (planetNode == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Planet body not found.");
            }

            var surfacePosition = planetNode.GetSurfacePosition(
                latDeg * (Math.PI / 180.0),
                lonDeg * (Math.PI / 180.0),
                ModApi.Flight.Sim.AltitudeType.AboveSeaLevel,
                0d,
                null);
            var height = planetNode.GetTerrainHeight(surfacePosition.normalized);

            var builder = new StringBuilder(256);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"height\":");
            AppendJsonValue(builder, height);
            builder.Append("}}");
            return builder.ToString();
        }

        private static string SerializeWorldCastRayResponse(int requestId, string json)
        {
            if (!TryParseWorldRayRequest(json, out var origin, out var direction, out var maxDistance))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid raycast arguments.");
            }

            var ray = new Ray(
                new Vector3((float)origin.x, (float)origin.y, (float)origin.z),
                new Vector3((float)direction.x, (float)direction.y, (float)direction.z).normalized);
            if (!Physics.Raycast(ray, out var hit, maxDistance))
            {
                return SerializeNullHitResponse(requestId);
            }

            var hitPart = ResolvePartScriptFromCollider(hit.collider);

            var builder = new StringBuilder(512);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"hit\":{");
            builder.Append("\"hit_type\":");
            AppendJsonValue(builder, hitPart != null ? "part" : "world");
            builder.Append(",\"distance\":");
            AppendJsonValue(builder, hit.distance);
            builder.Append(",\"point\":");
            AppendTelemetryVectorJson(builder, hit.point.x, hit.point.y, hit.point.z);
            builder.Append(",\"normal\":");
            AppendTelemetryVectorJson(builder, hit.normal.x, hit.normal.y, hit.normal.z);
            builder.Append(",\"part\":");
            if (hitPart?.Data != null)
            {
                AppendPartJson(builder, hitPart.Data, hitPart.CraftScript);
            }
            else
            {
                builder.Append("null");
            }

            builder.Append("}}}");
            return builder.ToString();
        }

        private static string SerializeNullHitResponse(int requestId)
        {
            var builder = new StringBuilder(128);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"hit\":null}}");
            return builder.ToString();
        }

        private static string SerializeGetActivationGroupResponse(int requestId, string json)
        {
            if (!TryReadJsonInt(json, "\"group\"", out var group) || group < 1)
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid get_activation_group arguments.");
            }

            var commandPod = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.ActiveCommandPod;
            var enabled = commandPod != null && commandPod.GetActivationGroupState(group);

            var builder = new StringBuilder(128);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"group\":");
            AppendJsonValue(builder, group);
            builder.Append(",\"enabled\":");
            AppendJsonValue(builder, enabled);
            builder.Append("}}");
            return builder.ToString();
        }

        private static string SerializeSetPartActivatedResponse(int requestId, string json)
        {
            if (!TryReadJsonInt(json, "\"part_id\"", out var partId) ||
                !TryReadJsonBool(json, "\"activated\"", out var activated))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid set_part_activated arguments.");
            }

            var craftId = TryReadJsonString(json, "\"craft_id\"", out var cid) ? cid : null;
            var craftScript = ResolveCraftScript(craftId);

            if (craftScript == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Craft not found or not active.");
            }

            var partScript = ResolvePartData(craftScript, partId)?.PartScript;
            if (partScript == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Part not found.");
            }

            partScript.Data.Activated = activated;

            var builder = new StringBuilder(128);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"part_id\":");
            AppendJsonValue(builder, partId);
            builder.Append(",\"activated\":");
            AppendJsonValue(builder, activated);
            builder.Append("}}");
            return builder.ToString();
        }

        private static string SerializeListPartsResponse(int requestId, string json)
        {
            if (!TryParseOptionalCraftRequest(json, out var craftId))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid list_parts arguments.");
            }

            var craftScript = ResolveCraftScript(craftId);
            if (craftScript?.Data?.Assembly == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Craft not found.");
            }

            var builder = new StringBuilder(2048);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"parts\":[");

            var first = true;
            foreach (ModApi.Craft.Parts.PartData partData in craftScript.Data.Assembly.Parts)
            {
                if (partData == null)
                {
                    continue;
                }

                if (!first)
                {
                    builder.Append(",");
                }

                AppendPartJson(builder, partData, craftScript);
                first = false;
            }

            builder.Append("]}}");
            return builder.ToString();
        }

        private static string SerializeGetPartResponse(int requestId, string json)
        {
            if (!TryParsePartLookupRequest(json, out var partId, out var craftId))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid get_part arguments.");
            }

            var craftScript = ResolveCraftScript(craftId);
            var partData = ResolvePartData(craftScript, partId);
            var builder = new StringBuilder(512);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"part\":");
            if (partData == null || craftScript == null)
            {
                builder.Append("null");
            }
            else
            {
                AppendPartJson(builder, partData, craftScript);
            }

            builder.Append("}}");
            return builder.ToString();
        }

        private static string SerializePartTransformResponse(int requestId, string json, bool localToPci)
        {
            if (!TryParsePartVectorRequest(json, out var partId, out var craftId, out var vector))
            {
                return SerializeRpcErrorResponse(requestId, "invalid_args", "Invalid part transform arguments.");
            }

            var craftScript = ResolveCraftScript(craftId);
            var partData = ResolvePartData(craftScript, partId);
            var partScript = partData?.PartScript;
            if (partScript?.Transform == null)
            {
                return SerializeRpcErrorResponse(requestId, "not_found", "Part not found.");
            }

            Vector3d result;
            if (localToPci)
            {
                var world = partScript.Transform.TransformPoint(new Vector3((float)vector.x, (float)vector.y, (float)vector.z));
                result = new Vector3d(world.x, world.y, world.z);
            }
            else
            {
                var local = partScript.Transform.InverseTransformPoint(new Vector3((float)vector.x, (float)vector.y, (float)vector.z));
                result = new Vector3d(local.x, local.y, local.z);
            }

            return SerializeVectorRpcResponse(requestId, result.x, result.y, result.z);
        }

        private static string SerializeRpcErrorResponse(int requestId, string code, string message)
        {
            var builder = new StringBuilder(256);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":false,\"error\":{\"code\":");
            AppendJsonValue(builder, code);
            builder.Append(",\"message\":");
            AppendJsonValue(builder, message);
            builder.Append("}}");
            return builder.ToString();
        }

        private static ModApi.Craft.ICraftScript ResolveCraftScript(string craftId)
        {
            var flightScene = ModApi.Common.Game.Instance.FlightScene;
            var activeCraftScript = flightScene?.CraftNode?.CraftScript;
            if (string.IsNullOrWhiteSpace(craftId))
            {
                return activeCraftScript;
            }

            if (!TryParseCraftId(craftId, out var nodeId))
            {
                return null;
            }

            var craftNodes = flightScene?.FlightState?.CraftNodes;
            if (craftNodes == null)
            {
                return null;
            }

            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                if (craftNode?.NodeId == nodeId)
                {
                    return craftNode.CraftScript;
                }
            }

            return null;
        }

        private static ModApi.Craft.Parts.PartData ResolvePartData(ModApi.Craft.ICraftScript craftScript, int partId)
        {
            return craftScript?.Data?.Assembly?.GetPartById(partId);
        }

        private static void AppendPartJson(
            StringBuilder builder,
            ModApi.Craft.Parts.PartData partData,
            ModApi.Craft.ICraftScript craftScript)
        {
            if (partData == null)
            {
                builder.Append("null");
                return;
            }

            var resolvedCraftScript = craftScript ?? partData.PartScript?.CraftScript;
            var craftNode = resolvedCraftScript?.CraftNode;
            var partScript = partData.PartScript;
            var worldPosition = partScript?.Transform != null
                ? new Vector3d(
                    partScript.Transform.position.x,
                    partScript.Transform.position.y,
                    partScript.Transform.position.z)
                : partData.Position;

            builder.Append("{\"part_id\":");
            AppendJsonValue(builder, partData.Id);
            builder.Append(",\"name\":");
            AppendJsonValue(builder, partData.Name);
            builder.Append(",\"craft_id\":");
            AppendJsonValue(builder, craftNode != null ? FormatCraftId(craftNode.NodeId) : null);
            builder.Append(",\"craft_name\":");
            AppendJsonValue(builder, craftNode != null ? GetCraftName(craftNode) : null);
            builder.Append(",\"part_type\":");
            AppendJsonValue(builder, partData.PartType?.Name);
            builder.Append(",\"mass\":");
            AppendJsonValue(builder, partData.Mass);
            builder.Append(",\"drag_scale\":");
            AppendJsonValue(builder, partData.DragScale);
            builder.Append(",\"temperature\":");
            AppendJsonValue(builder, partScript != null ? (object)partScript.Temperature : null);
            builder.Append(",\"activated\":");
            AppendJsonValue(builder, partData.Activated);
            builder.Append(",\"enabled\":");
            AppendJsonValue(builder, partData.Enabled);
            builder.Append(",\"destroyed\":");
            AppendJsonValue(builder, partData.IsDestroyed);
            builder.Append(",\"position\":");
            AppendTelemetryVectorJson(builder, worldPosition.x, worldPosition.y, worldPosition.z);
            builder.Append(",\"local_position\":");
            AppendTelemetryVectorJson(builder, partData.Position.x, partData.Position.y, partData.Position.z);
            builder.Append("}");
        }

        private static ModApi.Craft.Parts.IPartScript ResolvePartScriptFromCollider(Collider collider)
        {
            if (collider == null)
            {
                return null;
            }

            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            if (craftNodes == null)
            {
                return null;
            }

            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                var parts = craftNode?.CraftScript?.Data?.Assembly?.Parts;
                if (parts == null)
                {
                    continue;
                }

                foreach (ModApi.Craft.Parts.PartData partData in parts)
                {
                    var partScript = partData?.PartScript;
                    if (partScript == null)
                    {
                        continue;
                    }

                    if (ReferenceEquals(partScript.PrimaryCollider, collider))
                    {
                        return partScript;
                    }

                    var colliders = partScript.Colliders;
                    if (colliders == null)
                    {
                        continue;
                    }

                    foreach (ModApi.Craft.Parts.PartColliderScript partCollider in colliders)
                    {
                        if (ReferenceEquals(partCollider?.Collider, collider))
                        {
                            return partScript;
                        }
                    }
                }
            }

            return null;
        }

        private static ModApi.Flight.Sim.IPlanetNode ResolvePlanetNode(string bodyKey)
        {
            var normalized = string.IsNullOrWhiteSpace(bodyKey) ? null : bodyKey.Trim();
            ModApi.Flight.Sim.IPlanetNode activePlanet = null;
            var activeCraft = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript;
            activePlanet = activeCraft?.FlightData?.Orbit?.Parent;

            if (string.IsNullOrEmpty(normalized))
            {
                return activePlanet;
            }

            foreach (var planetNode in EnumeratePlanetNodes())
            {
                if (planetNode?.PlanetData == null)
                {
                    continue;
                }

                if (string.Equals(FormatBodyId(planetNode.PlanetData.Id), normalized, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(planetNode.PlanetData.Name, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return planetNode;
                }
            }

            if (activePlanet != null)
            {
                return activePlanet.FindPlanet(normalized);
            }

            return null;
        }

        private static IEnumerable<ModApi.Flight.Sim.IPlanetNode> EnumeratePlanetNodes()
        {
            var activePlanet = ModApi.Common.Game.Instance.FlightScene?.CraftNode?.CraftScript?.FlightData?.Orbit?.Parent;
            if (activePlanet == null)
            {
                yield break;
            }

            var root = activePlanet;
            while (root?.PlanetData?.Parent != null)
            {
                var parentName = root.PlanetData.Parent.Name;
                if (string.IsNullOrEmpty(parentName))
                {
                    break;
                }

                var parentNode = root.FindPlanet(parentName);
                if (parentNode == null || ReferenceEquals(parentNode, root))
                {
                    break;
                }

                root = parentNode;
            }

            var queue = new Queue<ModApi.Flight.Sim.IPlanetNode>();
            var emitted = new HashSet<string>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var planetNode = queue.Dequeue();
                if (planetNode?.PlanetData == null)
                {
                    continue;
                }

                var bodyId = FormatBodyId(planetNode.PlanetData.Id);
                if (!emitted.Add(bodyId))
                {
                    continue;
                }

                yield return planetNode;

                if (planetNode.ChildPlanets == null)
                {
                    continue;
                }

                foreach (ModApi.Flight.Sim.IPlanetNode child in planetNode.ChildPlanets)
                {
                    if (child != null)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        private static string FormatBodyId(Guid bodyId)
        {
            return bodyId.ToString("D");
        }

        private static void AppendBodyJson(StringBuilder builder, ModApi.Flight.Sim.IPlanetNode planetNode)
        {
            var planetData = planetNode?.PlanetData;
            var orbitData = planetData?.OrbitData;
            var parentBody = planetData?.Parent;
            var craftIds = GetCraftIdsForBody(planetNode);
            var childBodyIds = new List<string>();
            if (planetNode?.ChildPlanets != null)
            {
                foreach (ModApi.Flight.Sim.IPlanetNode child in planetNode.ChildPlanets)
                {
                    if (child?.PlanetData?.Id != null)
                    {
                        childBodyIds.Add(FormatBodyId(child.PlanetData.Id));
                    }
                }
            }

            builder.Append("{\"body_id\":");
            AppendJsonValue(builder, planetData != null ? FormatBodyId(planetData.Id) : string.Empty);
            builder.Append(",\"name\":");
            AppendJsonValue(builder, planetData?.Name ?? string.Empty);
            builder.Append(",\"parent_body_id\":");
            AppendJsonValue(builder, parentBody != null ? FormatBodyId(parentBody.Id) : null);
            builder.Append(",\"parent_body_name\":");
            AppendJsonValue(builder, parentBody?.Name);
            builder.Append(",\"child_body_ids\":[");
            for (var i = 0; i < childBodyIds.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                AppendJsonValue(builder, childBodyIds[i]);
            }

            builder.Append("],\"craft_ids\":[");
            for (var i = 0; i < craftIds.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                AppendJsonValue(builder, craftIds[i]);
            }

            builder.Append("],\"mass\":");
            AppendJsonValue(builder, planetData != null ? (object)planetData.Mass : null);
            builder.Append(",\"radius\":");
            AppendJsonValue(builder, planetData != null ? (object)planetData.Radius : null);
            builder.Append(",\"surface_gravity\":");
            AppendJsonValue(builder, planetData != null ? (object)planetData.SurfaceGravity : null);
            builder.Append(",\"sphere_of_influence\":");
            AppendJsonValue(builder, planetData != null ? (object)planetData.SphereOfInfluence : null);
            builder.Append(",\"angular_velocity\":");
            AppendJsonValue(builder, planetData != null ? (object)planetData.AngularVelocity : null);
            builder.Append(",\"has_water\":");
            AppendJsonValue(builder, planetData != null && planetData.HasWater);
            builder.Append(",\"has_terrain_physics\":");
            AppendJsonValue(builder, planetData != null && planetData.HasTerrainPhysics);
            builder.Append(",\"orbit_apoapsis\":null");
            builder.Append(",\"orbit_periapsis\":null");
            builder.Append(",\"orbit_period\":null");
            builder.Append(",\"orbit_inclination_rad\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.Inclination : null);
            builder.Append(",\"orbit_eccentricity\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.Eccentricity : null);
            builder.Append(",\"orbit_semi_major_axis\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.SemiMajorAxis : null);
            builder.Append(",\"orbit_semi_minor_axis\":");
            AppendJsonValue(
                builder,
                orbitData != null
                    ? (object)(orbitData.SemiMajorAxis * Math.Sqrt(Math.Max(0d, 1d - (orbitData.Eccentricity * orbitData.Eccentricity))))
                    : null);
            builder.Append(",\"orbit_true_anomaly\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.TrueAnomaly : null);
            builder.Append(",\"orbit_mean_anomaly\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.Time : null);
            builder.Append(",\"orbit_periapsis_argument\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.ArgumentOfPeriapsis : null);
            builder.Append(",\"orbit_right_ascension\":");
            AppendJsonValue(builder, orbitData != null ? (object)orbitData.RightAscensionOfAscendingNode : null);
            builder.Append("}");
        }

        private static List<string> GetCraftIdsForBody(ModApi.Flight.Sim.IPlanetNode planetNode)
        {
            var craftIds = new List<string>();
            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            if (craftNodes == null || planetNode?.PlanetData == null)
            {
                return craftIds;
            }

            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                var craftPlanet = craftNode?.CraftScript?.FlightData?.Orbit?.Parent?.PlanetData;
                if (craftPlanet == null
                    || !string.Equals(FormatBodyId(craftPlanet.Id), FormatBodyId(planetNode.PlanetData.Id), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                craftIds.Add(FormatCraftId(craftNode.NodeId));
            }

            return craftIds;
        }

        private static double ComputeAltitudeAboveSeaLevel(ModApi.Flight.Sim.IPlanetNode planetNode, Vector3d position)
        {
            var radius = planetNode?.PlanetData != null ? planetNode.PlanetData.Radius : 0d;
            return position.magnitude - radius;
        }

        private static double ComputeAltitudeAboveGroundLevel(ModApi.Flight.Sim.IPlanetNode planetNode, Vector3d position)
        {
            return ComputeAltitudeAboveSeaLevel(planetNode, position) - planetNode.GetTerrainHeight(position.normalized);
        }

        private static string SerializeVectorRpcResponse(int requestId, double x, double y, double z)
        {
            var builder = new StringBuilder(256);
            builder.Append("{\"type\":\"rpc_response\",\"id\":");
            builder.Append(requestId);
            builder.Append(",\"ok\":true,\"result\":{\"vector\":");
            AppendTelemetryVectorJson(builder, x, y, z);
            builder.Append("}}");
            return builder.ToString();
        }

        private IReadOnlyList<string> ApplyTrackedCraftIds(IEnumerable<string> craftIds)
        {
            var acceptedCraftIds = new List<string>();
            var acceptedNodeIds = new HashSet<int>();
            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            if (craftNodes == null)
            {
                TrackedCraftNodeIds.Clear();
                return acceptedCraftIds;
            }

            var knownNodeIds = new HashSet<int>();
            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                if (craftNode != null)
                {
                    knownNodeIds.Add(craftNode.NodeId);
                }
            }

            foreach (var craftId in craftIds)
            {
                if (!TryParseCraftId(craftId, out var nodeId) || !knownNodeIds.Contains(nodeId))
                {
                    continue;
                }

                if (acceptedNodeIds.Add(nodeId))
                {
                    acceptedCraftIds.Add(FormatCraftId(nodeId));
                }
            }

            TrackedCraftNodeIds.Clear();
            foreach (var nodeId in acceptedNodeIds)
            {
                TrackedCraftNodeIds.Add(nodeId);
            }

            return acceptedCraftIds;
        }

        private static string GetCraftName(ModApi.Craft.ICraftNode craftNode)
        {
            if (craftNode is ModApi.Flight.Sim.IOrbitNode orbitNode
                && !string.IsNullOrEmpty(orbitNode.Name))
            {
                return orbitNode.Name;
            }

            if (craftNode?.InitialCraftNodeData != null)
            {
                foreach (var initialCraftNodeData in craftNode.InitialCraftNodeData)
                {
                    if (!string.IsNullOrEmpty(initialCraftNodeData?.Name))
                    {
                        return initialCraftNodeData.Name;
                    }
                }
            }

            return string.Empty;
        }

        private static string FormatCraftId(int nodeId)
        {
            return $"craft_{nodeId}";
        }

        private static bool TryParseCraftId(string craftId, out int nodeId)
        {
            nodeId = 0;
            if (string.IsNullOrWhiteSpace(craftId))
            {
                return false;
            }

            if (craftId.StartsWith("craft_", StringComparison.Ordinal))
            {
                return int.TryParse(
                    craftId.Substring("craft_".Length),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out nodeId);
            }

            return int.TryParse(craftId, NumberStyles.Integer, CultureInfo.InvariantCulture, out nodeId);
        }

        private void RefreshEventSubscriptions(ModApi.Flight.IFlightScene flightScene, ModApi.Craft.ICraftScript craftScript)
        {
            if (!ReferenceEquals(_subscribedFlightScene, flightScene))
            {
                if (_subscribedFlightScene != null)
                {
                    _subscribedFlightScene.PlayerChangedSoi -= OnPlayerChangedSoi;
                }

                _subscribedFlightScene = flightScene;
                if (_subscribedFlightScene != null)
                {
                    _subscribedFlightScene.PlayerChangedSoi += OnPlayerChangedSoi;
                }
            }

            if (!ReferenceEquals(_subscribedCraftScript, craftScript))
            {
                if (_subscribedCraftScript != null)
                {
                    _subscribedCraftScript.DockComplete -= OnCraftDockComplete;
                }

                _subscribedCraftScript = craftScript;
                _observedDestroyedParts.Clear();
                _activeCollisionKeys.Clear();

                  if (_subscribedCraftScript != null)
                  {
                      _subscribedCraftScript.DockComplete += OnCraftDockComplete;
                      SeedObservedPartState(_subscribedCraftScript);
                      TrySeedObservedCollisionState(_subscribedCraftScript);
                  }
              }
          }

        private void PollActiveCraftEvents(ModApi.Craft.ICraftScript craftScript)
        {
            if (craftScript == null)
            {
                _observedDestroyedParts.Clear();
                _activeCollisionKeys.Clear();
                return;
              }

              PollDestroyedPartEvents(craftScript);
              TryPollCollisionEvents(craftScript);
          }

        private void SeedObservedPartState(ModApi.Craft.ICraftScript craftScript)
        {
            var parts = craftScript?.Data?.Assembly?.Parts;
            if (parts == null)
            {
                return;
            }

            foreach (ModApi.Craft.Parts.PartData partData in parts)
            {
                if (partData != null)
                {
                    _observedDestroyedParts[partData.Id] = partData.IsDestroyed;
                }
            }
        }

        private void PollDestroyedPartEvents(ModApi.Craft.ICraftScript craftScript)
        {
            var parts = craftScript?.Data?.Assembly?.Parts;
            if (parts == null)
            {
                _observedDestroyedParts.Clear();
                return;
            }

            var currentIds = new HashSet<int>();
            foreach (ModApi.Craft.Parts.PartData partData in parts)
            {
                if (partData == null)
                {
                    continue;
                }

                currentIds.Add(partData.Id);
                var previouslyDestroyed = _observedDestroyedParts.TryGetValue(partData.Id, out var knownDestroyed) && knownDestroyed;
                if (!previouslyDestroyed && partData.IsDestroyed)
                {
                    EnqueuePartExplodedEvent(craftScript, partData);
                }

                _observedDestroyedParts[partData.Id] = partData.IsDestroyed;
            }

            var removedIds = new List<int>();
            foreach (var observedPartId in _observedDestroyedParts.Keys)
            {
                if (!currentIds.Contains(observedPartId))
                {
                    removedIds.Add(observedPartId);
                }
            }

            foreach (var removedId in removedIds)
            {
                _observedDestroyedParts.Remove(removedId);
            }
        }

        private void SeedObservedCollisionState(ModApi.Craft.ICraftScript craftScript)
        {
            var collisions = craftScript?.Data?.Assembly?.PartCollisions;
            if (collisions == null)
            {
                return;
            }

            foreach (ModApi.Craft.Parts.IPartFlightCollision collision in collisions)
            {
                var key = BuildCollisionKey(collision);
                if (!string.IsNullOrEmpty(key))
                {
                    _activeCollisionKeys.Add(key);
                }
            }
        }

        private void TrySeedObservedCollisionState(ModApi.Craft.ICraftScript craftScript)
        {
            try
            {
                SeedObservedCollisionState(craftScript);
            }
            catch (Exception ex)
            {
                _activeCollisionKeys.Clear();
                Debug.LogWarning($"JunoIoBridge collision seeding skipped: {ex.Message}");
            }
        }

        private void PollCollisionEvents(ModApi.Craft.ICraftScript craftScript)
        {
            var collisions = craftScript?.Data?.Assembly?.PartCollisions;
            if (collisions == null)
            {
                _activeCollisionKeys.Clear();
                return;
            }

            var currentKeys = new HashSet<string>();
            foreach (ModApi.Craft.Parts.IPartFlightCollision collision in collisions)
            {
                var key = BuildCollisionKey(collision);
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                currentKeys.Add(key);
                if (!_activeCollisionKeys.Contains(key))
                {
                    EnqueueCollisionEvent(collision);
                }
            }

            _activeCollisionKeys.Clear();
            foreach (var key in currentKeys)
            {
                _activeCollisionKeys.Add(key);
            }
        }

        private void TryPollCollisionEvents(ModApi.Craft.ICraftScript craftScript)
        {
            try
            {
                PollCollisionEvents(craftScript);
            }
            catch (Exception ex)
            {
                _activeCollisionKeys.Clear();
                Debug.LogWarning($"JunoIoBridge collision polling skipped: {ex.Message}");
            }
        }

        private static string BuildCollisionKey(ModApi.Craft.Parts.IPartFlightCollision collision)
        {
            var partId = collision?.PartScript?.Data?.Id ?? -1;
            var otherPartId = collision?.OtherPartScript?.Data?.Id ?? -1;
            var isGround = collision != null && collision.IsGroundCollision ? 1 : 0;
            var otherCraftId = collision?.OtherPartScript?.CraftScript?.CraftNode != null
                ? FormatCraftId(collision.OtherPartScript.CraftScript.CraftNode.NodeId)
                : string.Empty;
            return $"{partId}:{otherPartId}:{isGround}:{otherCraftId}";
        }

        private void EnqueueCollisionEvent(ModApi.Craft.Parts.IPartFlightCollision collision)
        {
            var partScript = collision?.PartScript;
            var otherPartScript = collision?.OtherPartScript;
            var craftNode = partScript?.CraftScript?.CraftNode;
            var otherCraftNode = otherPartScript?.CraftScript?.CraftNode;
            var point = collision != null ? collision.Contact.point : Vector3.zero;
            var normal = collision != null ? collision.Contact.normal : Vector3.up;

            EnqueueEvent(new TelemetryEvent
            {
                event_type = "collision",
                time = Time.time,
                craft_id = craftNode != null ? FormatCraftId(craftNode.NodeId) : null,
                craft_name = craftNode != null ? GetCraftName(craftNode) : null,
                other_craft_id = otherCraftNode != null ? FormatCraftId(otherCraftNode.NodeId) : null,
                other_craft_name = otherCraftNode != null ? GetCraftName(otherCraftNode) : null,
                part_id = partScript?.Data != null ? (int?)partScript.Data.Id : null,
                part_name = partScript?.Data?.Name,
                other_part_id = otherPartScript?.Data != null ? (int?)otherPartScript.Data.Id : null,
                other_part_name = otherPartScript?.Data?.Name,
                parent_body = BuildBodyRef(partScript?.CraftScript?.FlightData?.Orbit?.Parent?.PlanetData),
                relative_velocity = collision != null ? (float?)collision.RelativeVelocityMagnitude : null,
                impulse = collision != null ? (float?)collision.Impulse : null,
                is_ground_collision = collision?.IsGroundCollision,
                point = CreateTelemetryVector3(new Vector3d(point.x, point.y, point.z)),
                normal = CreateTelemetryVector3(new Vector3d(normal.x, normal.y, normal.z)),
            });
        }

        private void EnqueuePartExplodedEvent(ModApi.Craft.ICraftScript craftScript, ModApi.Craft.Parts.PartData partData)
        {
            var craftNode = craftScript?.CraftNode;
            EnqueueEvent(new TelemetryEvent
            {
                event_type = "part_exploded",
                time = Time.time,
                craft_id = craftNode != null ? FormatCraftId(craftNode.NodeId) : null,
                craft_name = craftNode != null ? GetCraftName(craftNode) : null,
                part_id = partData != null ? (int?)partData.Id : null,
                part_name = partData?.Name,
                parent_body = BuildBodyRef(craftScript?.FlightData?.Orbit?.Parent?.PlanetData),
            });
        }

        private void OnCraftDockComplete(string playerCraftName, int playerNodeId, string otherCraftName, int otherNodeId)
        {
            EnqueueEvent(new TelemetryEvent
            {
                event_type = "craft_docked",
                time = Time.time,
                craft_id = playerNodeId >= 0 ? FormatCraftId(playerNodeId) : null,
                craft_name = playerCraftName,
                other_craft_id = otherNodeId >= 0 ? FormatCraftId(otherNodeId) : null,
                other_craft_name = otherCraftName,
                parent_body = BuildBodyRef(_subscribedCraftScript?.FlightData?.Orbit?.Parent?.PlanetData),
            });
        }

        private void OnPlayerChangedSoi(ModApi.Craft.ICraftNode playerCraftNode, ModApi.Flight.Sim.IPlanetNode newParent)
        {
            EnqueueEvent(new TelemetryEvent
            {
                event_type = "entered_soi",
                time = Time.time,
                craft_id = playerCraftNode != null ? FormatCraftId(playerCraftNode.NodeId) : null,
                craft_name = playerCraftNode != null ? GetCraftName(playerCraftNode) : null,
                new_parent_body = BuildBodyRef(newParent?.PlanetData),
            });
        }

        private void HandleMessageCommand(PendingMessageCommand command, ModApi.Craft.ICraftNode senderCraftNode)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.scope) || string.IsNullOrWhiteSpace(command.message))
            {
                return;
            }

            var normalizedScope = command.scope.Trim().ToLowerInvariant();
            switch (normalizedScope)
            {
                case "broadcast":
                    BroadcastMessageToAllCrafts(senderCraftNode, command.message, command.data_json);
                    break;
                case "craft":
                    if (!string.IsNullOrWhiteSpace(command.craft_id))
                    {
                        QueueCraftMessage(command.craft_id, senderCraftNode, command.message, command.data_json, "craft");
                    }

                    break;
                case "nearby":
                    BroadcastMessageToNearbyCrafts(
                        senderCraftNode,
                        command.message,
                        command.data_json,
                        command.radius ?? DefaultNearbyMessageRadius);
                    break;
            }
        }

        private void BroadcastMessageToAllCrafts(ModApi.Craft.ICraftNode senderCraftNode, string messageName, string dataJson)
        {
            foreach (var craftNode in EnumerateAllCraftNodes())
            {
                if (craftNode == null)
                {
                    continue;
                }

                if (senderCraftNode != null && craftNode.NodeId == senderCraftNode.NodeId)
                {
                    continue;
                }

                QueueCraftMessage(FormatCraftId(craftNode.NodeId), senderCraftNode, messageName, dataJson, "broadcast");
            }
        }

        private void BroadcastMessageToNearbyCrafts(
            ModApi.Craft.ICraftNode senderCraftNode,
            string messageName,
            string dataJson,
            float radius)
        {
            if (senderCraftNode == null || radius <= 0f)
            {
                return;
            }

            var senderPosition = ToVector3d(senderCraftNode.Position);
            var radiusSquared = (double)radius * radius;
            foreach (var craftNode in EnumerateAllCraftNodes())
            {
                if (craftNode == null || craftNode.NodeId == senderCraftNode.NodeId)
                {
                    continue;
                }

                var delta = ToVector3d(craftNode.Position) - senderPosition;
                if (delta.sqrMagnitude > radiusSquared)
                {
                    continue;
                }

                QueueCraftMessage(FormatCraftId(craftNode.NodeId), senderCraftNode, messageName, dataJson, "nearby");
            }
        }

        private IEnumerable<ModApi.Craft.ICraftNode> EnumerateAllCraftNodes()
        {
            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            if (craftNodes == null)
            {
                yield break;
            }

            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                if (craftNode != null)
                {
                    yield return craftNode;
                }
            }
        }

        private void QueueCraftMessage(
            string recipientCraftId,
            ModApi.Craft.ICraftNode senderCraftNode,
            string messageName,
            string dataJson,
            string deliveryScope)
        {
            if (string.IsNullOrWhiteSpace(recipientCraftId) || string.IsNullOrWhiteSpace(messageName))
            {
                return;
            }

            _pendingCraftMessageEvents.Add(new TelemetryEvent
            {
                event_type = "message_received",
                time = Time.time,
                craft_id = recipientCraftId,
                sender_craft_id = senderCraftNode != null ? FormatCraftId(senderCraftNode.NodeId) : null,
                sender_craft_name = senderCraftNode != null ? GetCraftName(senderCraftNode) : null,
                message_name = messageName,
                message_data_json = string.IsNullOrWhiteSpace(dataJson) ? "null" : dataJson,
                delivery_scope = deliveryScope,
                parent_body = BuildBodyRef(senderCraftNode?.CraftScript?.FlightData?.Orbit?.Parent?.PlanetData),
            });
        }

        private void EnqueueEvent(TelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null)
            {
                return;
            }

            telemetryEvent.event_id = $"{telemetryEvent.event_type}_{++_eventSequence}";
            _pendingEvents.Enqueue(telemetryEvent);
        }

        private List<TelemetryEvent> DrainPendingEvents()
        {
            var events = new List<TelemetryEvent>(_pendingEvents.Count);
            while (_pendingEvents.Count > 0)
            {
                events.Add(_pendingEvents.Dequeue());
            }

            return events;
        }

        private List<TelemetryEvent> DrainPendingCraftMessages(string activeCraftId, string activeCraftName)
        {
            var events = new List<TelemetryEvent>();
            if (string.IsNullOrWhiteSpace(activeCraftId) || _pendingCraftMessageEvents.Count == 0)
            {
                return events;
            }

            for (var i = _pendingCraftMessageEvents.Count - 1; i >= 0; i--)
            {
                var telemetryEvent = _pendingCraftMessageEvents[i];
                if (!string.Equals(telemetryEvent?.craft_id, activeCraftId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(telemetryEvent.craft_name))
                {
                    telemetryEvent.craft_name = activeCraftName;
                }

                events.Add(telemetryEvent);
                _pendingCraftMessageEvents.RemoveAt(i);
            }

            events.Reverse();
            return events;
        }

        private string SerializeTelemetryEnvelope(ModApi.Craft.ICraftNode activeCraftNode)
        {
            var builder = new StringBuilder(8192);
            var activeCraftScript = activeCraftNode?.CraftScript;
            var target = BuildTelemetryTarget(activeCraftScript?.FlightData?.NavSphereTarget);
            var activeCraftId = FormatCraftId(activeCraftNode.NodeId);
            var activeCraftName = GetCraftName(activeCraftNode);
            var pendingEvents = DrainPendingEvents();
            pendingEvents.AddRange(DrainPendingCraftMessages(activeCraftId, activeCraftName));
            builder.Append("{\"type\":\"telemetry\",\"seq\":");
            builder.Append(++_telemetrySequence);
            builder.Append(",\"t\":");
            AppendJsonValue(builder, Time.time);
            builder.Append(",\"active_craft_id\":");
            AppendJsonValue(builder, activeCraftId);
            builder.Append(",\"crafts\":{");

            var first = true;
            foreach (var craftNode in EnumerateTelemetryCraftNodes(activeCraftNode))
            {
                if (craftNode == null)
                {
                    continue;
                }

                if (!first)
                {
                    builder.Append(",");
                }

                var craftId = FormatCraftId(craftNode.NodeId);
                var state = BuildTelemetryState(craftNode);

                AppendJsonString(builder, craftId);
                builder.Append(":{\"craft_id\":");
                AppendJsonValue(builder, craftId);
                builder.Append(",\"name\":");
                AppendJsonValue(builder, GetCraftName(craftNode));
                builder.Append(",\"state\":");
                SerializeTelemetryState(builder, state);
                builder.Append("}");
                first = false;
            }

            builder.Append("},\"target\":");
            SerializeTelemetryTarget(builder, target);
            builder.Append(",\"events\":[");
            for (var i = 0; i < pendingEvents.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                AppendTelemetryEventJson(builder, pendingEvents[i]);
            }

            builder.Append("]");
            builder.Append("}");
            return builder.ToString();
        }

        private IEnumerable<ModApi.Craft.ICraftNode> EnumerateTelemetryCraftNodes(ModApi.Craft.ICraftNode activeCraftNode)
        {
            var emittedNodeIds = new HashSet<int>();
            if (activeCraftNode != null && emittedNodeIds.Add(activeCraftNode.NodeId))
            {
                yield return activeCraftNode;
            }

            var craftNodes = ModApi.Common.Game.Instance.FlightScene?.FlightState?.CraftNodes;
            if (craftNodes == null)
            {
                yield break;
            }

            foreach (ModApi.Craft.ICraftNode craftNode in craftNodes)
            {
                if (craftNode == null || craftNode.IsPlayer)
                {
                    continue;
                }

                if (!TrackedCraftNodeIds.Contains(craftNode.NodeId))
                {
                    continue;
                }

                if (emittedNodeIds.Add(craftNode.NodeId))
                {
                    yield return craftNode;
                }
            }
        }

        private static void SerializeTelemetryState(StringBuilder builder, TelemetryState state)
        {
            builder.Append("{");

            for (var i = 0; i < TelemetryStateFields.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                var field = TelemetryStateFields[i];
                builder.Append("\"");
                builder.Append(field.Name);
                builder.Append("\":");
                AppendJsonValue(builder, field.GetValue(state));
            }

            builder.Append("}");
        }

        private static void SerializeTelemetryTarget(StringBuilder builder, TelemetryTarget target)
        {
            if (target == null)
            {
                builder.Append("null");
                return;
            }

            builder.Append("{\"target_type\":");
            AppendJsonValue(builder, target.target_type);
            builder.Append(",\"target_id\":");
            AppendJsonValue(builder, target.target_id);
            builder.Append(",\"name\":");
            AppendJsonValue(builder, target.name);
            builder.Append(",\"parent_body_id\":");
            AppendJsonValue(builder, target.parent_body != null ? target.parent_body.body_id : null);
            builder.Append(",\"parent_body_name\":");
            AppendJsonValue(builder, target.parent_body != null ? target.parent_body.name : null);
            builder.Append(",\"position\":");
            SerializeTelemetryVector3(builder, target.position);
            builder.Append(",\"velocity\":");
            SerializeTelemetryVector3(builder, target.velocity);
            builder.Append(",\"craft_id\":");
            AppendJsonValue(builder, target.craft_id);
            builder.Append(",\"part_id\":");
            AppendJsonValue(builder, target.part_id);
            builder.Append(",\"part_name\":");
            AppendJsonValue(builder, target.part_name);
            builder.Append(",\"parent_craft_id\":");
            AppendJsonValue(builder, target.parent_craft_id);
            builder.Append(",\"parent_craft_name\":");
            AppendJsonValue(builder, target.parent_craft_name);
            builder.Append(",\"body_id\":");
            AppendJsonValue(builder, target.body_id);
            builder.Append(",\"radius\":");
            AppendJsonValue(builder, target.radius);
            builder.Append(",\"mass\":");
            AppendJsonValue(builder, target.mass);
            builder.Append(",\"mu\":");
            AppendJsonValue(builder, target.mu);
            builder.Append(",\"angular_velocity\":");
            AppendJsonValue(builder, target.angular_velocity);
            builder.Append(",\"landmark_id\":");
            AppendJsonValue(builder, target.landmark_id);
            builder.Append(",\"latitude_deg\":");
            AppendJsonValue(builder, target.latitude_deg);
            builder.Append(",\"longitude_deg\":");
            AppendJsonValue(builder, target.longitude_deg);
            builder.Append("}");
        }

        private static void AppendTelemetryEventJson(StringBuilder builder, TelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null)
            {
                builder.Append("null");
                return;
            }

            builder.Append("{\"event_type\":");
            AppendJsonValue(builder, telemetryEvent.event_type);
            builder.Append(",\"event_id\":");
            AppendJsonValue(builder, telemetryEvent.event_id);
            builder.Append(",\"time\":");
            AppendJsonValue(builder, telemetryEvent.time);
            builder.Append(",\"craft_id\":");
            AppendJsonValue(builder, telemetryEvent.craft_id);
            builder.Append(",\"craft_name\":");
            AppendJsonValue(builder, telemetryEvent.craft_name);
            builder.Append(",\"sender_craft_id\":");
            AppendJsonValue(builder, telemetryEvent.sender_craft_id);
            builder.Append(",\"sender_craft_name\":");
            AppendJsonValue(builder, telemetryEvent.sender_craft_name);
            builder.Append(",\"other_craft_id\":");
            AppendJsonValue(builder, telemetryEvent.other_craft_id);
            builder.Append(",\"other_craft_name\":");
            AppendJsonValue(builder, telemetryEvent.other_craft_name);
            builder.Append(",\"part_id\":");
            AppendJsonValue(builder, telemetryEvent.part_id);
            builder.Append(",\"part_name\":");
            AppendJsonValue(builder, telemetryEvent.part_name);
            builder.Append(",\"other_part_id\":");
            AppendJsonValue(builder, telemetryEvent.other_part_id);
            builder.Append(",\"other_part_name\":");
            AppendJsonValue(builder, telemetryEvent.other_part_name);
            builder.Append(",\"message_name\":");
            AppendJsonValue(builder, telemetryEvent.message_name);
            builder.Append(",\"message_data\":");
            AppendRawJsonValue(builder, telemetryEvent.message_data_json);
            builder.Append(",\"delivery_scope\":");
            AppendJsonValue(builder, telemetryEvent.delivery_scope);
            builder.Append(",\"parent_body_id\":");
            AppendJsonValue(builder, telemetryEvent.parent_body != null ? telemetryEvent.parent_body.body_id : null);
            builder.Append(",\"parent_body_name\":");
            AppendJsonValue(builder, telemetryEvent.parent_body != null ? telemetryEvent.parent_body.name : null);
            builder.Append(",\"new_parent_body\":");
            if (telemetryEvent.new_parent_body == null)
            {
                builder.Append("null");
            }
            else
            {
                builder.Append("{\"body_id\":");
                AppendJsonValue(builder, telemetryEvent.new_parent_body.body_id);
                builder.Append(",\"name\":");
                AppendJsonValue(builder, telemetryEvent.new_parent_body.name);
                builder.Append("}");
            }

            builder.Append(",\"relative_velocity\":");
            AppendJsonValue(builder, telemetryEvent.relative_velocity);
            builder.Append(",\"impulse\":");
            AppendJsonValue(builder, telemetryEvent.impulse);
            builder.Append(",\"is_ground_collision\":");
            AppendJsonValue(builder, telemetryEvent.is_ground_collision);
            builder.Append(",\"point\":");
            SerializeTelemetryVector3(builder, telemetryEvent.point);
            builder.Append(",\"normal\":");
            SerializeTelemetryVector3(builder, telemetryEvent.normal);
            builder.Append("}");
        }

        private static void SerializeTelemetryVector3(StringBuilder builder, TelemetryVector3 vector)
        {
            if (vector == null)
            {
                builder.Append("null");
                return;
            }

            builder.Append("{\"x\":");
            AppendJsonValue(builder, vector.x);
            builder.Append(",\"y\":");
            AppendJsonValue(builder, vector.y);
            builder.Append(",\"z\":");
            AppendJsonValue(builder, vector.z);
            builder.Append("}");
        }

        private static void AppendRawJsonValue(StringBuilder builder, string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                builder.Append("null");
                return;
            }

            builder.Append(rawJson);
        }

        private static void AppendTelemetryVectorJson(StringBuilder builder, double x, double y, double z)
        {
            builder.Append("{\"x\":");
            AppendJsonValue(builder, x);
            builder.Append(",\"y\":");
            AppendJsonValue(builder, y);
            builder.Append(",\"z\":");
            AppendJsonValue(builder, z);
            builder.Append("}");
        }

        private static void AppendJsonValue(StringBuilder builder, object value)
        {
            switch (value)
            {
                case null:
                    builder.Append("null");
                    break;
                case string stringValue:
                    AppendJsonString(builder, stringValue);
                    break;
                case bool boolValue:
                    builder.Append(boolValue ? "true" : "false");
                    break;
                case float floatValue:
                    builder.Append(float.IsFinite(floatValue)
                        ? floatValue.ToString("R", CultureInfo.InvariantCulture)
                        : "0");
                    break;
                case double doubleValue:
                    builder.Append(double.IsFinite(doubleValue)
                        ? doubleValue.ToString("R", CultureInfo.InvariantCulture)
                        : "0");
                    break;
                case sbyte or byte or short or ushort or int or uint or long or ulong:
                    builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                    break;
                default:
                    if (value is IFormattable formattable)
                    {
                        builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
                        break;
                    }

                    AppendJsonString(builder, value.ToString());
                    break;
            }
        }

        private static void AppendJsonString(StringBuilder builder, string value)
        {
            builder.Append("\"");

            foreach (var character in value)
            {
                switch (character)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        if (character < ' ')
                        {
                            builder.Append("\\u");
                            builder.Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            builder.Append(character);
                        }

                        break;
                }
            }

            builder.Append("\"");
        }

        private void DisposeClients()
        {
            if (_subscribedCraftScript != null)
            {
                _subscribedCraftScript.DockComplete -= OnCraftDockComplete;
                _subscribedCraftScript = null;
            }

            if (_subscribedFlightScene != null)
            {
                _subscribedFlightScene.PlayerChangedSoi -= OnPlayerChangedSoi;
                _subscribedFlightScene = null;
            }

            _observedDestroyedParts.Clear();
            _activeCollisionKeys.Clear();
            _pendingEvents.Clear();
            _pendingCraftMessageEvents.Clear();

            if (_telemetryClient != null)
            {
                _telemetryClient.Close();
                _telemetryClient.Dispose();
                _telemetryClient = null;
            }

            if (_commandClient != null)
            {
                _commandClient.Close();
                _commandClient.Dispose();
                _commandClient = null;
            }

            if (_rpcRequestClient != null)
            {
                _rpcRequestClient.Close();
                _rpcRequestClient.Dispose();
                _rpcRequestClient = null;
            }

            if (_rpcResponseClient != null)
            {
                _rpcResponseClient.Close();
                _rpcResponseClient.Dispose();
                _rpcResponseClient = null;
            }
        }
    }
}
