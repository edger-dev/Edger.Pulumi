namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using PersistentVolumeClaim = global::Pulumi.Kubernetes.Core.V1.PersistentVolumeClaim;

public enum StorageClass {
    LocalPath,
}

public enum AccessMode {
    ReadWriteOnce,
    ReadonlyMany,
    ReadWriteMany,
}

public static class StorageClassExtention {
    public static string ToStorageClassName(this StorageClass v) {
        switch (v) {
            case StorageClass.LocalPath:
                return "local-path";
            default:
                return "N/A";
        }
    }
}


public partial class K8s {
    public static PersistentVolumeClaimSpecArgs PvcSpec(
        string requestSize = "1Gi",
        StorageClass storageClass = StorageClass.LocalPath,
        AccessMode accessMode = AccessMode.ReadWriteOnce
    ) {
        return new PersistentVolumeClaimSpecArgs {
            AccessModes = new InputList<string> {
                accessMode.ToString()
            },
            StorageClassName = storageClass.ToStorageClassName(),
            Resources = new ResourceRequirementsArgs {
                Requests = new InputMap<string> {
                    { "storage", requestSize }
                }
            }
        };
    }
    public static PersistentVolumeClaimArgs Pvc(
        Namespace ns, string name,
        InputMap<string>? labels = null,
        string requestSize = "1Gi",
        StorageClass? storageClass = null
    ) {
        return new PersistentVolumeClaimArgs {
            Metadata = K8s.ObjectMeta(ns, name, labels),
            Spec = PvcSpec(requestSize, storageClass ?? StorageClass.LocalPath),
        };
    }

    public static VolumeArgs PvcVolume(string name, string claimName, bool readOnly = false) {
        var source = new PersistentVolumeClaimVolumeSourceArgs {
            ClaimName = claimName,
            ReadOnly = readOnly,
        };
        return new VolumeArgs {
            Name = name,
            PersistentVolumeClaim = source,
        };
    }
}

public static class PersistentVolumeClaimArgsExtension {
    public static PersistentVolumeClaim Apply(this PersistentVolumeClaimArgs args, string name) {
        return new PersistentVolumeClaim(name, args);
    }
}