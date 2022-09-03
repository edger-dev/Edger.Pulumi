namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

public partial class K8s {
    public static PodSpecArgs PodSpec(
        ContainerArgs container,
        InputList<VolumeArgs>? volumes = null
    ) {
        var spec = new PodSpecArgs {
            Containers = { container },
        };
        if (volumes != null) {
            spec.Volumes = volumes;
        }
        return spec;
    }

    public static PodTemplateSpecArgs PodTemplateSpec   (
        Namespace ns,
        InputMap<string> labels,
        ContainerArgs container,
        InputList<VolumeArgs>? volumes = null,
        InputMap<string>? annotations = null
    ) => new PodTemplateSpecArgs {
        Metadata = K8s.ObjectMeta(ns, labels:labels, annotations:annotations),
        Spec = PodSpec(container, volumes),
    };
}
