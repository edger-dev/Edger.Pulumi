namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

public partial class K8s {
    public static SecurityContextArgs SecurityContext(
        Input<bool>? privileged = null,
        Input<bool>? allowPrivilegeEscalation = null
    ) {
        var args = new SecurityContextArgs();
        if (privileged != null) {
            args.Privileged = privileged;
        }
        if (allowPrivilegeEscalation != null) {
            args.AllowPrivilegeEscalation = allowPrivilegeEscalation;
        }
        return args;
    }
}