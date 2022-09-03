namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Service = global::Pulumi.Kubernetes.Core.V1.Service;

public enum ServiceType {
    ClusterIP,
    LoadBalancer,
}

public partial class K8s {
    public const string ServiceType_ClusterIP = "ClusterIP";

    public static ServicePortArgs ServicePort(string name, int port, int targetPort) => new ServicePortArgs {
        Name = name,
        Port = port,
        TargetPort = targetPort,
    };

    public static ServiceSpecArgs ServiceSpec(
        InputMap<string> labels,
        ServicePortArgs[] ports,
        ServiceType serviceType = ServiceType.ClusterIP
    ) => new ServiceSpecArgs {
        Type = serviceType.ToString(),
        Ports = ports,
        Selector = labels,
    };

    public static ServiceArgs Service(
        Namespace ns, string name,
        InputMap<string> labels,
        ServicePortArgs[] ports,
        ServiceType serviceType = ServiceType.ClusterIP
    ) {
        return new ServiceArgs {
            Metadata = K8s.ObjectMeta(ns, name, labels),
            Spec = ServiceSpec(labels, ports, serviceType),
        };
    }
}

public static class ServiceArgsExtension {
    public static Service Apply(this ServiceArgs args, string name) {
        return new Service(name, args);
    }
}

public static class ServiceExtension {
    public static Output<string> ClusterIP(this Service service) {
        return service.Spec.Apply(spec => spec.ClusterIP);
    }

    public static Output<string> LoadBalancerIP(this Service service) {
        return service.Status.Apply(status => status.LoadBalancer.Ingress[0].Ip ?? status.LoadBalancer.Ingress[0].Hostname);
    }
}