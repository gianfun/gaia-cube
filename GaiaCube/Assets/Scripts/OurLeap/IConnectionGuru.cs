using UnityEngine;
using System.Collections;

public interface IConnectionGuru {
    string GetIP();
    bool ShouldUseWebLeap();
    bool ShouldUseLeap();
}
