namespace Edger.Pulumi;

using global::Pulumi;

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