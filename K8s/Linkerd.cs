namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

public partial class K8s {
    public static InputMap<string> LinkerdAnnotations {
        get => new InputMap<string> {
            { "linkerd.io/inject", "enabled" },
        };
    }
}
