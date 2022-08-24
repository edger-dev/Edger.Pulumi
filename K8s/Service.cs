namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;

public enum ServiceType {
    ClusterIP,
}

public partial class K8s {
    public const string ServiceType_ClusterIP = "ClusterIP";

    public static ServicePortArgs ServicePort(string name, int port, int targetPort) => new ServicePortArgs {
        Name = name,
        Port = port,
        TargetPort = targetPort,
    };

    public static ServiceSpecArgs ServiceSpec(
        Deployment deployment,
        ServicePortArgs[] ports,
        ServiceType serviceType = ServiceType.ClusterIP
    ) => new ServiceSpecArgs {
        Type = serviceType.ToString(),
        Ports = ports,
        Selector = deployment.Spec.Apply(s => s.Template.Metadata.Labels),
    };

    public static ServiceArgs Service(
        Deployment deployment,
        ServicePortArgs[] ports,
        ServiceType serviceType = ServiceType.ClusterIP
    ) {
        return new ServiceArgs {
            Metadata = new ObjectMetaArgs {
                Namespace = deployment.Metadata.Apply(m => m.Namespace),
                Name = deployment.Metadata.Apply(m => m.Name),
                Labels = deployment.Metadata.Apply(m => m.Labels),
            },
            Spec = ServiceSpec(deployment, ports, serviceType),
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
}