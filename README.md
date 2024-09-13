<br/>
<div align="center">
<h3 align="center">Komi</h3>
<p align="center">
Working in progress
</p>
</div>

[Join our Discord](https://discord.gg/AhUnkUyCDe)

## Development

- There is `enet-native` dependency that needs to be build for your os ( Windows should already been provided )
```bash
cd enet-native
cmake -S . -DBUILD_SHARED_LIBS=ON -B ./build
cmake --build ./build --config Release
```
- ENet.Managed as the ffi for the native library
- Run dotnet run in the project root directory

## Todo
- [ ] Implement bot library
- [x] Implement GUI
- [ ] Item database GUI
- [x] Inventory GUI
- [x] World serializer ( Not Full )
- [x] items.dat parser
- [x] World map
- [ ] Find path
- [x] Session token
- [x] Save account & session to JSON
- [x] Legacy login
- [ ] Auto collect
- [ ] Google login
- [x] Steam login
- [ ] Apple login
- [ ] To be added
