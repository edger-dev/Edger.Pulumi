namespace Edger.Pulumi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::Pulumi;
using global::Pulumi.Kubernetes.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using global::Pulumi.Kubernetes.Types.Inputs.ApiExtensions.V1Beta1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;

public class ClusterApp {
    public readonly Deployment Deployment;
    public readonly Service Service;
    public readonly Output<string> IP;

    public ClusterApp(Deployment deployment, Service service) {
        Deployment = deployment;
        Service = service;
        IP = service.ClusterIP();
    }

    public ClusterApp(Namespace ns, string name, string image, string portName, int portNumber) {
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber)
        });
        Deployment = K8s.Deployment(ns, name, container).Apply(name);
        Service = K8s.Service(Deployment, new[] {
            K8s.ServicePort(portName, portNumber, portNumber),
        }).Apply(name);
        IP = Service.ClusterIP();
    }
}