# UnityJoyconRehab
Este repositorio contiene la versión pública del prototipo del videojuego desarrollado para COANIQUEM, orientado a la rehabilitación de muñeca usando controles Joy-Con de Nintendo Switch y la librería joyconlib.

## Instalación del Juego

1. Descargue el archivo .zip más reciente desde la sección "Releases".
2. Extraiga el contenido en una carpeta de su preferencia.
3. Conecte ambos Joy-Con al computador.
4. Inicie el juego.

## Conexión de los Joy-Con

En el repositorio encontrará un archivo PDF llamado `ConexionJoycons.pdf` con la guía paso a paso para conectar los Joy-Con por Bluetooth.


## Visualización y análisis de datos

Para usar el script `ViewData.py` debes tener Python instalado y crear un entorno virtual. Luego instala las librerías necesarias usando el archivo `requirements.txt` que se encuentra en la carpeta `Assets/Scripts`.

```bash
python -m venv venv
cd .venv
cd Scripts
.\Activate
cd ../..
cd Assets/Scripts
pip install -r requirements.txt
```

Después ejecuta el script:

```bash
python ViewData.py
```

## Cómo convertir el script en un ejecutable (.exe)

Si deseas generar un archivo ejecutable de `ViewData.py` para que pueda usarse sin instalar Python ni librerías, sigue estos pasos (dentro de la carpeta que contiene el script `ViewData.py`):

1. Instala PyInstaller en tu entorno de Python (solo en caso de que no se haya instalado desde el requirements.txt):
   ```bash
   pip install pyinstaller
   ```

2. Genera el ejecutable con el siguiente comando:
   ```bash
   pyinstaller --onefile ViewData.py
   ```

3. Al finalizar, se crearán las carpetas `dist` y `build` en el directorio del proyecto.  
   El archivo ejecutable estará en la carpeta `dist`.

En este repositorio, ya se encuentra el ejecutable listo para usar en la carpeta `dist` (Assets/Scripts/dist).
Solo debes hacer doble clic en `ViewData.exe`. Al abrirlo, aparecerá primero una ventana negra (consola) que indica que el programa está cargando. Luego se abrirá la ventana principal de la aplicación, donde podrás realizar el análisis de los archivos JSON generados por el videojuego.

---