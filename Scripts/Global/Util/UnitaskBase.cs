using System.Threading;
using UnityEngine;

public class UnitaskBase : MonoBehaviour
{
    CancellationTokenSource _disableCancellation = new(); //비활성화시 취소처리
    CancellationTokenSource _destroyCancellation = new(); //삭제시 취소처리

    private void OnEnable() {
        if (_disableCancellation != null) {
            _disableCancellation.Dispose();
        }
        _disableCancellation = new CancellationTokenSource();
    }

    private void OnDisable() => _disableCancellation.Cancel();
    
    private void OnDestroy() {
        _destroyCancellation.Cancel();
        _destroyCancellation.Dispose();
    }
}
