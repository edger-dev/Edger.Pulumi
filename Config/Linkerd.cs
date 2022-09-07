namespace Edger.Pulumi.Config;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Rbac.V1;
using ClusterRoleBinding = global::Pulumi.Kubernetes.Rbac.V1.ClusterRoleBinding;

public class Linkerd {
    public const string Namepace__Linkerd_Viz = "linkerd_viz";
    public const string ClusterRole__Linkerd_Viz_Prometheus = "linkerd-linkerd-viz-prometheus";

    public static InputMap<string> InjectAnnotations {
        get => new InputMap<string> {
            { "linkerd.io/inject", "enabled" },
        };
    }

    public static RoleRefArgs VizPrometheusRoleRef() {
        return K8s.RoleRef(ClusterRole__Linkerd_Viz_Prometheus);
    }

    public static ClusterRoleBinding PrometheusFederateRoleBinding(
        Namespace ns,
        string name = "prometheus-federate",
        string serviceAccountName = "default"
    ) {
        var subject = K8s.Subject(ns, serviceAccountName);
        return K8s.ClusterRoleBinding(
            ns, name,
            VizPrometheusRoleRef(),
            subject
        ).Apply(name);
    }
}
