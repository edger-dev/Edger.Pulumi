namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class BytebaseDB : Postgres {
    public new const string NAME = "bytebase-db";

    public const string DB = "bytebase";
    public const string USER = "bytebase";

    public BytebaseDB(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string password,
        string user = USER,
        string db = DB,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, image, pvc, password, user, db, lbPort:lbPort, name:name
    ) {
    }
}