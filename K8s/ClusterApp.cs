namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public abstract class ClusterApp : BaseApp {
    public readonly Service Service;
    public readonly Output<string> IP;

    protected ClusterApp(
        Namespace ns, string name,
        string portName, int portNumber
    ) : base(ns, name) {
        Service = K8s.Service(ns, name, Labels, new[] {
            K8s.ServicePort(portName, portNumber, portNumber),
        }).Apply(name);
        IP = Service.ClusterIP();
    }

    protected ClusterApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2
    ) : base(ns, name) {
        Service = K8s.Service(ns, name, Labels, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
        }).Apply(name);
        IP = Service.ClusterIP();
    }

    protected ClusterApp(
        Namespace ns, string name,
        string portName1, int portNumber1,
        string portName2, int portNumber2,
        string portName3, int portNumber3
    ) : base(ns, name) {
        Service = K8s.Service(ns, name, Labels, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
            K8s.ServicePort(portName3, portNumber3, portNumber3),
        }).Apply(name);
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
