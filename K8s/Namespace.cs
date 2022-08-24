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

public class Namespace {
    public readonly global::Pulumi.Kubernetes.Core.V1.Namespace Real;

    public Output<string> Value {
        get {
            return Real.Metadata.Apply(m => m.Name);
        }
    }

    public Namespace(string ns) {
        Real = new global::Pulumi.Kubernetes.Core.V1.Namespace(ns);
    }
}