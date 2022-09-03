namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;


using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;

public partial class K8s {
    public static DeploymentSpecArgs DeploymentSpec(
        Namespace ns, string name,
        InputMap<string> labels,
        ContainerArgs container,
        InputList<VolumeArgs>? podVolumes = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) {
        return new DeploymentSpecArgs {
            Selector = new LabelSelectorArgs {
                MatchLabels = labels,
            },
            Replicas = replicas,
            Template = PodTemplateSpec(ns, labels, container, podVolumes, podAnnotations),
        };
    }

    public static DeploymentArgs Deployment(
        Namespace ns, string name,
        InputMap<string> labels,
        ContainerArgs container,
        InputList<VolumeArgs>? podVolumes = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) {
        var metadata = K8s.ObjectMeta(ns, name, labels);
        return new DeploymentArgs {
            Metadata = metadata,
            Spec = DeploymentSpec(ns, name, labels, container, podVolumes, podAnnotations, replicas),
        };
    }
}

public static class DeploymentArgsExtension {
    public static Deployment Apply(this DeploymentArgs args, string name) {
        return new Deployment(name, args);
    }
}