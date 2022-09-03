namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class StatelessApp : ClusterApp {
    public readonly Deployment Deployment;

    public StatelessApp(
        Namespace ns, string name,
        string portName, int portNumber,
        string image,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name, portName, portNumber) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber)
        }, env, args, command);
        Deployment = K8s.Deployment(ns, name, Labels, container, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }

    public StatelessApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string image,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) : base (ns, name, portName1, portNumber1, portName2, portNumber2) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
        }, env, args, command);
        Deployment = K8s.Deployment(ns, name, Labels, container, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }

    public StatelessApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3,
        string image,
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
        }, env, args, command);
        Deployment = K8s.Deployment(ns, name, Labels, container, podAnnotations:podAnnotations, replicas:replicas).Apply(name);
    }
}
