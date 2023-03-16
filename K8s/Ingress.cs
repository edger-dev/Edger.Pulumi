namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Networking.V1;

using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;

public enum IngressPathType {
    Prefix,
    Exact,
}

public partial class K8s {
    public static ServiceBackendPortArgs ServiceBackendPort(string portName) => new ServiceBackendPortArgs {
        Name = portName,
    };

    public static ServiceBackendPortArgs ServiceBackendPort(int portNumber) => new ServiceBackendPortArgs {
        Number = portNumber,
    };

    public static IngressBackendArgs IngressBackend(string serviceName, ServiceBackendPortArgs servicePort) => new IngressBackendArgs {
        Service = new IngressServiceBackendArgs {
            Name = serviceName,
            Port = servicePort,
        },
    };

    public static IngressBackendArgs IngressBackend(string serviceName, string portName) => new IngressBackendArgs {
        Service = new IngressServiceBackendArgs {
            Name = serviceName,
            Port = ServiceBackendPort(portName),
        },
    };

    public static IngressBackendArgs IngressBackend(string serviceName, int portNumber) => new IngressBackendArgs {
        Service = new IngressServiceBackendArgs {
            Name = serviceName,
            Port = ServiceBackendPort(portNumber),
        },
    };

    public static HTTPIngressPathArgs HTTPIngressPath(
        IngressBackendArgs backend,
        string path = "/",
        IngressPathType pathType = IngressPathType.Prefix
    ) => new HTTPIngressPathArgs {
        Backend = backend,
        Path = path,
        PathType = pathType.ToString(),
    };

    public static IngressRuleArgs IngressRule(string host) => new IngressRuleArgs {
        Host = host,
    };

    public static HTTPIngressRuleValueArgs HTTPIngressRuleValue(HTTPIngressPathArgs[] paths) => new HTTPIngressRuleValueArgs {
        Paths = paths,
    };

    public static IngressRuleArgs IngressRule(HTTPIngressRuleValueArgs http) => new IngressRuleArgs {
        Http = http,
    };

    public static IngressRuleArgs IngressRule(string host, HTTPIngressRuleValueArgs http) => new IngressRuleArgs {
        Host = host,
        Http = http,
    };

    public static IngressSpecArgs IngressSpec(IngressRuleArgs[] rules) => new IngressSpecArgs {
        Rules = rules,
    };

    public static IngressArgs Ingress(Namespace ns, string name, IngressSpecArgs spec, InputMap<string>? annotations = null) {
        var metadata = K8s.ObjectMeta(ns, name);
        if (annotations != null) {
            metadata.Annotations = annotations;
        }
        return new IngressArgs {
            Metadata = metadata,
            Spec = spec,
        };
    }
}

public static class IngressArgsExtension {
    public static Ingress Apply(this IngressArgs args, string name) {
        return new Ingress(name, args);
    }
}
