namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

public partial class K8s {
    public static ObjectMetaArgs ObjectMeta(
        Namespace ns,
        string? name = null,
        InputMap<string>? labels = null,
        InputMap<string>? annotations = null
    ) {
        var metadata = new ObjectMetaArgs {
            Namespace = ns.Value,
        };
        if (name != null) {
            metadata.Name = name;
        }
        if (labels != null) {
            metadata.Labels = labels;
        }
        if (annotations != null) {
            metadata.Annotations = annotations;
        }
        return metadata;
    }
}

