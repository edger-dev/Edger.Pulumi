namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using ConfigMap = global::Pulumi.Kubernetes.Core.V1.ConfigMap;

public abstract class Volume {
    public readonly Namespace Namespace;
    public readonly string Name;
    public readonly string MountPath;
    public readonly string MountName;

    protected Volume(
        Namespace ns, string name,
        string mountPath,
        string mountName
    ) {
        Namespace = ns;
        Name = name;
        MountPath = mountPath;
        MountName = mountName;
    }

    public static InputList<PersistentVolumeClaimArgs> FilterPvcTemplates(Volume[] volumes) {
        var result = new InputList<PersistentVolumeClaimArgs>();
        foreach (var volume in volumes) {
            if (volume is PvcTemplateVolume) {
                result.Add(((PvcTemplateVolume)volume).Pvc);
            }
        }
        return result;
    }

    public static InputList<VolumeArgs> FilterVolumes(Volume[] volumes) {
        var result = new InputList<VolumeArgs>();
        foreach (var volume in volumes) {
            if (volume is ConfigMapVolume) {
                result.Add(((ConfigMapVolume)volume).Volume);
            }
        }
        return result;
    }

    public static InputList<VolumeMountArgs> ToVolumeMounts(Volume[] volumes) {
        var result = new InputList<VolumeMountArgs>();
        foreach (var volume in volumes) {
            result.Add(K8s.ContainerVolumeMount(volume.MountName, volume.MountPath));
        }
        return result;
    }
}

public class ConfigMapVolume : Volume {
    public readonly ConfigMap ConfigMap;
    public readonly VolumeArgs Volume;
    public ConfigMapVolume(
        Namespace ns, string name,
        string mountPath,
        string mountName,
        params (string, string)[] entries
    ) : base(ns, name, mountPath, mountName) {
        var data = new InputMap<string>();
        foreach (var entry in entries) {
            data.Add(entry.Item1, entry.Item2);
        }
        ConfigMap = K8s.ConfigMap(ns, name, data).Apply(name);
        Volume = K8s.ConfigMapVolume(MountName, ConfigMap);
    }
}

public class PvcTemplateVolume : Volume {
    public readonly PersistentVolumeClaimArgs Pvc;

    public PvcTemplateVolume(
        Namespace ns, string name,
        string mountPath,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) : base(ns, name, mountPath, name) {
        Pvc = K8s.PersistentVolumeClaim(ns, name, labels, requestSize, storageClass);
    }
}