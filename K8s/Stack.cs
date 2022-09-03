namespace Edger.Pulumi;

using System.Threading.Tasks;
using global::Pulumi;

public class K8sStack : global::Pulumi.Stack {
    public readonly Namespace Namespace;
    public K8sStack(string ns, StackOptions? options = null) : base(options) {
        Namespace = new Namespace(ns);
    }

    public Pvc Pvc(
        string pvcName,
        string requestSize = "10Gi",
        StorageClass? storageClass = null
    ) {
        return new Pvc(Namespace, pvcName, requestSize:requestSize, storageClass:storageClass);
    }

    public static Task<int> RunAsync<T>()
        where T : Stack, new()
        => global::Pulumi.Deployment.RunAsync<T>();
}