namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using ConfigMap = global::Pulumi.Kubernetes.Core.V1.ConfigMap;

public partial class K8s {
    public static ConfigMapArgs ConfigMap(
        Namespace ns, string name,
        InputMap<string> data,
        bool immutable = false
    ) {
        return new ConfigMapArgs {
            Metadata = K8s.ObjectMeta(ns, name),
            Data = data,
            Immutable = immutable,
        };
    }

    public static ConfigMapArgs BinaryConfigMap(
        Namespace ns, string name,
        InputMap<string> binaryData,
        bool immutable = false
    ) {
        return new ConfigMapArgs {
            Metadata = K8s.ObjectMeta(ns, name),
            BinaryData = binaryData,
            Immutable = immutable,
        };
    }

    public static VolumeArgs ConfigMapVolume(string name, ConfigMap configMap, bool readOnly = false) {
        var source = new ConfigMapVolumeSourceArgs {
            Name = configMap.Metadata.Apply(m => m.Name),
        };
        return new VolumeArgs {
            Name = name,
            ConfigMap = source,
        };
    }
}

public static class ConfigMapArgsExtension {
    public static ConfigMap Apply(this ConfigMapArgs args, string name) {
        return new ConfigMap(name, args);
    }
}