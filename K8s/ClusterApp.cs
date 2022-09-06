namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public abstract class ClusterApp : BaseApp {
    public Service? Service { get; private set; }
    public Output<string>? IP { get; private set; }

    protected ClusterApp(
        Namespace ns, string name
    ) : base(ns, name) {
    }

    protected void SetupService(
        string portName, int portNumber
    ) {
        Service = K8s.Service(Namespace, Name, Labels, new[] {
            K8s.ServicePort(portName, portNumber, portNumber),
        }).Apply(Name);
        IP = Service.ClusterIP();
    }

    protected void SetupService(
        string portName1, int portNumber1,
        string portName2, int portNumber2
    ) {
        Service = K8s.Service(Namespace, Name, Labels, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
        }).Apply(Name);
        IP = Service.ClusterIP();
    }

    protected void SetupService(
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3
    ) {
        Service = K8s.Service(Namespace, Name, Labels, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
            K8s.ServicePort(portName3, portNumber3, portNumber3),
        }).Apply(Name);
        IP = Service.ClusterIP();
    }

    protected void SetupService(
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3,
        string portName4, int portNumber4
    ) {
        Service = K8s.Service(Namespace, Name, Labels, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
            K8s.ServicePort(portName3, portNumber3, portNumber3),
            K8s.ServicePort(portName4, portNumber4, portNumber4),
        }).Apply(Name);
        IP = Service.ClusterIP();
    }

    public Service ApplyLoadBalancer(string lbName, int lbPort, int servicePort) {
        return K8s.Service(
            Namespace, lbName, Labels, new [] {
                K8s.ServicePort(lbName, lbPort, servicePort),
            },
            serviceType: ServiceType.LoadBalancer
        ).Apply(lbName);
    }

    public Ingress ApplyHostIngress(string ingressHost, int servicePort) {
        return K8s.Ingress(Namespace, ingressHost, K8s.IngressSpec(new [] {
            K8s.IngressRule(ingressHost, K8s.HTTPIngressRuleValue(new [] {
                K8s.HTTPIngressPath(K8s.IngressBackend(Name, servicePort))
            })),
        })).Apply(ingressHost);
    }

}
