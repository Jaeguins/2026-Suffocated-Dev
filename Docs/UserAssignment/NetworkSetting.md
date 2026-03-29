# NGO 네트워크 초기 설정 (에디터 작업)

> 작성: 2026-03-30
> NGO 패키지 설치 완료 후 에디터에서 수행해야 할 셋팅 목록

---

## 1. NetworkManager 오브젝트 생성

- [ ] 씬(또는 DontDestroyOnLoad 전용 씬)에 빈 GameObject를 생성하고 이름을 `NetworkManager`로 지정
- [ ] 해당 오브젝트에 `NetworkManager` 컴포넌트 추가
  - Component 메뉴: `Netcode > NetworkManager`

---

## 2. Transport 설정

- [ ] NetworkManager 인스펙터의 **Network Transport** 필드에 `UnityTransport` 컴포넌트 할당
  - 동일 오브젝트에 `Unity Transport` 컴포넌트를 추가한 뒤 드래그 연결
- [ ] UnityTransport의 **Protocol Type** 확인
  - 로컬 테스트: `Unity Transport (UDP)` 유지
  - Relay 사용 시: 추후 `Relay Unity Transport`으로 변경 예정 (todo 참고)

---

## 3. NetworkObject 프리팹 등록

- [ ] 네트워크로 동기화할 프리팹(플레이어 캐릭터 등)에 `NetworkObject` 컴포넌트 추가
- [ ] NetworkManager 인스펙터의 **Network Prefabs List** 에 해당 프리팹들을 등록
  - 기본 생성된 `DefaultNetworkPrefabs` 에셋을 사용하거나 새 리스트 에셋 생성 가능
  - `Assets/DefaultNetworkPrefabs.asset` 이 이미 존재하면 해당 에셋 활용

---

## 4. 씬 관리 설정

- [ ] NetworkManager 인스펙터에서 **Enable Scene Management** 옵션 확인
  - 심리스 로딩 구조(Additive 씬 전환)를 사용하므로 **활성화 권장**
  - 활성화 시 NGO가 씬 로드/언로드를 네트워크 전체에 동기화함

---

## 5. Player Prefab 지정

- [ ] NetworkManager 인스펙터의 **Player Prefab** 필드에 플레이어 캐릭터 프리팹 할당
  - 플레이어 프리팹 미완성 시 임시 프리팹으로 연결 후 교체 가능

---

## 6. 동작 확인 (로컬 테스트)

- [ ] `SceneTest` 씬에서 NetworkManager 오브젝트를 배치하고 Play Mode 진입
- [ ] 코드 또는 임시 버튼으로 `NetworkManager.Singleton.StartHost()` 호출하여 호스트 기동 확인
- [ ] 두 번째 에디터 인스턴스(또는 빌드)에서 `StartClient()`로 접속 확인

---

## 참고

- Relay 서버 연동은 **세션 검색 방식**이 결정된 후 별도 작업 예정 (`todo.md` > 네트워크 항목 참고)
- 호스트 마이그레이션 처리 방식 미결 — 결정 전까지 호스트 이탈 시 세션 종료로 처리
