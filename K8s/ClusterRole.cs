namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Rbac.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using ClusterRoleBinding = global::Pulumi.Kubernetes.Rbac.V1.ClusterRoleBinding;

public enum RoleKind {
    ClusterRole,
}

public enum SubjectKind {
    ServiceAccount,
    User,
    Group,
}

public partial class K8s {
    public const string ApiGroup__Authorization = "rbac.authorization.k8s.io";

    public static RoleRefArgs RoleRef(
        string name,
        string apiGroup = ApiGroup__Authorization,
        RoleKind kind = RoleKind.ClusterRole
    ) => new RoleRefArgs {
        ApiGroup = apiGroup,
        Kind = kind.ToString(),
        Name = name,
    };

    public static SubjectArgs Subject(
        Namespace ns,
        string name,
        SubjectKind kind = SubjectKind.ServiceAccount,
        string? apiGroup = null
    ) {
        var result = new SubjectArgs {
            Namespace = ns.Value,
            Kind = kind.ToString(),
            Name = name,
        };
        if (apiGroup != null) {
            result.ApiGroup = apiGroup;
        }
        return result;
    }

    public static ClusterRoleBindingArgs ClusterRoleBinding(
        Namespace ns,
        string name,
        RoleRefArgs role,
        params SubjectArgs[] subjects
    ) {
        return new ClusterRoleBindingArgs {
            Metadata = K8s.ObjectMeta(ns, name),
            RoleRef = role,
            Subjects = subjects,
        };
    }
}

public static class ClusterRoleBindingArgsExtension {
    public static ClusterRoleBinding Apply(this ClusterRoleBindingArgs args, string name) {
        return new ClusterRoleBinding(name, args);
    }
}