namespace Edger.Pulumi;

using System.IO;
using System.Threading.Tasks;
using global::Pulumi;

public class K8sStack : global::Pulumi.Stack {
    public readonly Namespace Namespace;
    public K8sStack(string ns, StackOptions? options = null) : base(options) {
        Namespace = new Namespace(ns);
    }

    public PvcTemplateVolume PvcTemplateVolume(
        string pvcName,
        string mountPath,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(
            Namespace, pvcName, mountPath,
            requestSize, labels,
            storageClass:storageClass
        );
    }

    public string LoadStringFromFile(string path) {
        StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        string str = reader.ReadToEnd();
        reader.Close();
        return str;
    }

    public static Task<int> RunAsync<T>()
        where T : Stack, new()
        => global::Pulumi.Deployment.RunAsync<T>();
}