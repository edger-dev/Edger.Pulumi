namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using StatefulSet = global::Pulumi.Kubernetes.Apps.V1.StatefulSet;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class StatefulApp : ClusterApp {
    public readonly StatefulSet StatefulSet;

    public StatefulApp(
        Namespace ns, string name,
        string portName, int portNumber,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcs,
        InputList<VolumeArgs>? podVolumes,
        InputList<VolumeMountArgs> volumeMounts,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name, portName, portNumber) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber)
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcs, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }

    public StatefulApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcs,
        InputList<VolumeArgs>? podVolumes,
        InputList<VolumeMountArgs> volumeMounts,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name, portName1, portNumber1, portName2, portNumber2) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcs, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }

    public StatefulApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3,
        string image,
        InputList<PersistentVolumeClaimArgs> pvcs,
        InputList<VolumeArgs>? podVolumes,
        InputList<VolumeMountArgs> volumeMounts,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name, portName1, portNumber1, portName2, portNumber2, portName3, portNumber3) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
            K8s.ContainerPort(portNumber3),
        }, env, args, command, volumeMounts);
        StatefulSet = K8s.StatefulSet(ns, name, Labels, container, pvcs, podVolumes, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }
}
