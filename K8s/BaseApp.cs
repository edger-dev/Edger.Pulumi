namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public abstract class BaseApp {
    public readonly Namespace Namespace;
    public readonly string Name;
    public readonly InputMap<string> Labels;

    public static InputMap<string> AppLabels(string name) {
        return new InputMap<string> {
            { "app", name },
        };
    }

    protected BaseApp(
        Namespace ns, string name
    ) {
        Namespace = ns;
        Name = name;
        Labels = AppLabels(name);
    }
}
