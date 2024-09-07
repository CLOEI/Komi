<br/>
<div align="center">
<h3 align="center">Komi</h3>
<p align="center">
Working in progress
</p>
</div>

## Development

- There is `enet-native` dependency that needs to be build for your os ( Windows should already been provided )
```bash
cd enet-native
cmake -S . -DBUILD_SHARED_LIBS=ON -B ./build
cmake --build ./build --config Release
```
- ENet.Managed as the ffi for the native library
- Run dotnet run in the root directory

## Todo
- [ ] Implement bot library
- [ ] Implement GUI
- [ ] To be added