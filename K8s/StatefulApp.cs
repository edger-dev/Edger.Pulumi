namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

using StatefulSet = global::Pulumi.Kubernetes.Apps.V1.StatefulSet;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using PersistentVolumeClaim = global::Pulumi.Kubernetes.Core.V1.PersistentVolumeClaim;

public class StatefulApp : ClusterApp {
    public readonly StatefulSet StatefulSet;

    protected static InputList<PersistentVolumeClaimArgs> GetPvcTemplates(
        Pvc pvc
    ) => new InputList<PersistentVolumeClaimArgs> {
        pvc.Args,
    };

    protected static InputList<VolumeMountArgs> GetVolumeMounts(
        string mountName,
        string mountPath
    ) => new InputList<VolumeMountArgs> {
        K8s.ContainerVolume(mountName, mountPath),
    };

    protected static InputList<VolumeMountArgs> GetVolumeMounts(
        string mountName1,
        string mountPath1,
        string mountName2,
        string mountPath2
    ) => new InputList<VolumeMountArgs> {
        K8s.ContainerVolume(mountName1, mountPath1),
        K8s.ContainerVolume(mountName2, mountPath2),
    };

    protected static InputList<VolumeMountArgs> GetVolumeMounts(
        string mountName1,
        string mountPath1,
        string mountName2,
        string mountPath2,
        string mountName3,
        string mountPath3
    ) => new InputList<VolumeMountArgs> {
        K8s.ContainerVolume(mountName1, mountPath1),
        K8s.ContainerVolume(mountName2, mountPath2),
        K8s.ContainerVolume(mountName3, mountPath3),
    };

    public StatefulApp(
        Namespace ns, string name,
        string portName, int portNumber,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcTemplates,
        InputList<VolumeMountArgs> volumeMounts,
        InputList<VolumeArgs>? podVolumes = null,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber)
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcTemplates, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
        SetupService(portName, portNumber);
    }

    public StatefulApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcTemplates,
        InputList<VolumeMountArgs> volumeMounts,
        InputList<VolumeArgs>? podVolumes = null,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcTemplates, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
        SetupService(portName1, portNumber1, portName2, portNumber2);
    }

    public StatefulApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcTemplates,
        InputList<VolumeMountArgs> volumeMounts,
        InputList<VolumeArgs>? podVolumes = null,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
            K8s.ContainerPort(portNumber3),
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcTemplates, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
        SetupService(portName1, portNumber1, portName2, portNumber2, portName3, portNumber3);
    }
    public StatefulApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3,
        string portName4, int portNumber4,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcTemplates,
        InputList<VolumeMountArgs> volumeMounts,
        InputList<VolumeArgs>? podVolumes = null,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
            K8s.ContainerPort(portNumber3),
            K8s.ContainerPort(portNumber4),
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcTemplates, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
        SetupService(portName1, portNumber1, portName2, portNumber2, portName3, portNumber3, portName4, portNumber4);
    }
}
