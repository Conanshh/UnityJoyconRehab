import json
import csv
import tkinter as tk
from tkinter import filedialog, messagebox
import matplotlib.pyplot as plt
import pandas as pd

# Global variable to store last CSV path
last_csv_file = None

def parse_json_to_csv(json_data, output_csv):
    all_rows = []

    # Flexión-Extensión
    for session_idx, session in enumerate(json_data.get("flexionExtension", [])):
        for move_idx, move in enumerate(session["movements"]):
            angle = move.get("Angle", None)
            threshold = move.get("thresholdAngle", None)
            timestamp = move.get("timeStamp", None)
            row = {
                "userName": json_data.get("userName", ""),
                "date": json_data.get("date", ""),
                "highScoreFE": json_data.get("highScoreFE", ""),
                "highScoreAbAd": json_data.get("highScoreAbAd", ""),
                "type": "Flexión-Extensión",
                "sessionIndex": session_idx,
                "movementIndex": move_idx,
                "movementType": move.get("movementType", ""),
                "Angle": angle,
                "thresholdAngle": threshold,
                "timestamp": timestamp,
                "joyconUsed": session.get("joyconUsed", ""),
                "axisUsed": session.get("axisUsed", ""),
                "assignedGameTime": session.get("assignedGameTime", ""),
                "totalMovements": session.get("totalMovements", ""),
                "totalGameTimeFE": session.get("totalGameTimeFE", ""),
                "totalGameTimeAbAd": session.get("totalGameTimeAbAd", ""),
                "rewardCount": session.get("rewardCount", ""),
                "characterID": session.get("characterID", "")
            }
            all_rows.append(row)

    # Abducción-Aducción
    for session_idx, session in enumerate(json_data.get("abduccionAduccion", [])):
        for move_idx, move in enumerate(session["movements"]):
            angle = move.get("Angle", None)
            threshold = move.get("thresholdAngle", None)
            timestamp = move.get("timeStamp", None)
            row = {
                "userName": json_data.get("userName", ""),
                "date": json_data.get("date", ""),
                "highScoreFE": json_data.get("highScoreFE", ""),
                "highScoreAbAd": json_data.get("highScoreAbAd", ""),
                "type": "Abducción-Aducción",
                "sessionIndex": session_idx,
                "movementIndex": move_idx,
                "movementType": move.get("movementType", ""),
                "Angle": angle,
                "thresholdAngle": threshold,
                "timestamp": timestamp,
                "joyconUsed": session.get("joyconUsed", ""),
                "axisUsed": session.get("axisUsed", ""),
                "assignedGameTime": session.get("assignedGameTime", ""),
                "totalMovements": session.get("totalMovements", ""),
                "totalGameTimeFE": session.get("totalGameTimeFE", ""),
                "totalGameTimeAbAd": session.get("totalGameTimeAbAd", ""),
                "rewardCount": session.get("rewardCount", ""),
                "characterID": session.get("characterID", "")
            }
            all_rows.append(row)

    keys = all_rows[0].keys()
    with open(output_csv, 'w', newline='', encoding='utf-8-sig') as output_file:
        dict_writer = csv.DictWriter(output_file, fieldnames=keys, delimiter=';')
        dict_writer.writeheader()
        dict_writer.writerows(all_rows)

# Graficar CSV
# Muestra los ángulos por tipo de movimiento y sesión.
def plot_angles_by_type(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    for tipo in ["Flexión-Extensión", "Abducción-Aducción"]:
        sub = df[df["type"] == tipo]
        sesiones = sub["sessionIndex"].unique()
        plt.figure(figsize=(8,4))
        for sesion in sesiones:
            sub_sesion = sub[sub["sessionIndex"] == sesion]
            plt.plot(sub_sesion["movementIndex"], sub_sesion["Angle"], marker='o', label=f"Sesión {sesion+1}")
        plt.title(f"Ángulo por movimiento - {tipo}")
        plt.xlabel("Índice de movimiento")
        plt.ylabel("Ángulo (°)")
        plt.legend()
        plt.grid(True)
        plt.tight_layout()
        plt.show()

# Muestra la cantidad de movimientos y el tiempo total de juego por tipo.
# Tomando el tiempo maximo de juego de Flexión-Extensión y Abducción-Aducción.
# Y contando los movimientos de todas las sesiones por tipo.
def plot_movements_and_time_comparison(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    resumen = df.groupby("type").agg(
        movimientos=('movementIndex', 'count'),
        tiempo_FE=('totalGameTimeFE', 'max'),
        tiempo_AbAd=('totalGameTimeAbAd', 'max')
    ).reset_index()

    # Selecciona el tiempo correcto para cada tipo
    resumen['tiempo'] = resumen.apply(
        lambda row: row['tiempo_FE'] if row['type'] == "Flexión-Extensión" else row['tiempo_AbAd'], axis=1
    )

    x = range(len(resumen))
    width = 0.35

    fig, ax1 = plt.subplots()
    ax2 = ax1.twinx()
    ax1.bar([i - width/2 for i in x], resumen['movimientos'], width, label='Movimientos', color='cornflowerblue')
    ax2.bar([i + width/2 for i in x], resumen['tiempo'], width, label='Tiempo (s)', color='mediumseagreen')

    ax1.set_ylabel('Cantidad de movimientos')
    ax2.set_ylabel('Tiempo total (s)')
    ax1.set_xticks(x)
    ax1.set_xticklabels(resumen['type'])
    ax1.set_title('Comparación de movimientos y tiempo por tipo')
    fig.legend(loc="upper right")
    plt.tight_layout()
    plt.show()

# Muestra la cantidad de movimientos y el tiempo total de juego por sesión.
def plot_movements_and_time_by_session(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    resumen = df.groupby(["type", "sessionIndex"]).agg(
        movimientos=('movementIndex', 'count'),
        tiempo_FE=('totalGameTimeFE', 'max'),
        tiempo_AbAd=('totalGameTimeAbAd', 'max')
    ).reset_index()

    resumen['tiempo'] = resumen.apply(
        lambda row: row['tiempo_FE'] if row['type'] == "Flexión-Extensión" else row['tiempo_AbAd'], axis=1
    )

    for tipo in resumen['type'].unique():
        sub = resumen[resumen['type'] == tipo]
        plt.figure(figsize=(8,4))
        plt.bar(sub['sessionIndex'], sub['movimientos'], width=0.4, label='Movimientos', color='cornflowerblue')
        plt.bar(sub['sessionIndex'] + 0.4, sub['tiempo'], width=0.4, label='Tiempo (s)', color='mediumseagreen')
        plt.xlabel('Índice de sesión')
        plt.ylabel('Cantidad / Tiempo')
        plt.title(f'Comparación por sesión - {tipo}')
        plt.legend()
        plt.tight_layout()
        plt.show()

# Función para graficar ángulos vs tiempo para Flexión-Extensión
def plot_angle_vs_time_flexion_extension(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    sub = df[df["type"] == "Flexión-Extensión"]
    sesiones = sub["sessionIndex"].unique()
    plt.figure(figsize=(8,4))
    for sesion in sesiones:
        sub_sesion = sub[sub["sessionIndex"] == sesion]
        # Solo graficar si hay datos y timestamps válidos
        if not sub_sesion["timestamp"].isnull().all():
            plt.plot(sub_sesion["timestamp"], sub_sesion["Angle"], marker='o', label=f"Sesión {sesion+1}")
    plt.title("Ángulo vs Tiempo - Flexión-Extensión")
    plt.xlabel("Tiempo (s)")
    plt.ylabel("Ángulo (°)")
    plt.legend()
    plt.grid(True)
    plt.tight_layout()
    plt.show()

# Función para graficar ángulos vs tiempo para Abducción-Aducción
def plot_angle_vs_time_abduccion_aduccion(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    sub = df[df["type"] == "Abducción-Aducción"]
    sesiones = sub["sessionIndex"].unique()
    plt.figure(figsize=(8,4))
    for sesion in sesiones:
        sub_sesion = sub[sub["sessionIndex"] == sesion]
        # Solo graficar si hay datos y timestamps válidos
        if not sub_sesion["timestamp"].isnull().all():
            plt.plot(sub_sesion["timestamp"], sub_sesion["Angle"], marker='o', label=f"Sesión {sesion+1}")
    plt.title("Ángulo vs Tiempo - Abducción-Aducción")
    plt.xlabel("Tiempo (s)")
    plt.ylabel("Ángulo (°)")
    plt.legend()
    plt.grid(True)
    plt.tight_layout()
    plt.show()

# Función para graficar tiempo asignado vs tiempo real por sesión
def plot_assigned_vs_real_time_by_session(csv_file):
    df = pd.read_csv(csv_file, encoding='utf-8-sig', delimiter=';')
    for tipo in df['type'].unique():
        sub = df[df['type'] == tipo]
        resumen = sub.groupby('sessionIndex').agg(
            assigned_time=('assignedGameTime', 'max'),
            real_time=('totalGameTimeFE', 'max') if tipo == "Flexión-Extensión" else ('totalGameTimeAbAd', 'max')
        ).reset_index()
        x = resumen['sessionIndex']
        width = 0.35
        plt.figure(figsize=(8,4))
        plt.bar(x - width/2, resumen['assigned_time'], width, label='Tiempo asignado', color='skyblue')
        plt.bar(x + width/2, resumen['real_time'], width, label='Tiempo real', color='orange')
        plt.xlabel('Índice de sesión')
        plt.ylabel('Tiempo (s)')
        plt.title(f'Tiempo asignado vs real por sesión - {tipo}')
        plt.xticks(x)
        plt.legend()
        plt.tight_layout()
        plt.show()

# Función para graficar el último CSV cargado o convertido
def plot_last_csv():
    if last_csv_file:
        try:
            plot_angles_by_type(last_csv_file)
            plot_movements_and_time_comparison(last_csv_file)
            plot_movements_and_time_by_session(last_csv_file)
            plot_angle_vs_time_flexion_extension(last_csv_file)
            plot_angle_vs_time_abduccion_aduccion(last_csv_file)
            plot_assigned_vs_real_time_by_session(last_csv_file)
        except Exception as e:
            messagebox.showerror("Error", f"Error al graficar el archivo CSV:\n{e}")
    else:
        messagebox.showwarning("¡Atención!", "Primero debes cargar o convertir un archivo CSV.")

# Funciones para cargar JSON y CSV, y graficar los datos
def load_json_file():
    global last_csv_file
    file_path = filedialog.askopenfilename(filetypes=[("JSON files", "*.json")])
    if not file_path:
        return

    with open(file_path, 'r', encoding='utf-8') as f:
        json_data = json.load(f)

    output_csv = file_path.replace(".json", ".csv")
    parse_json_to_csv(json_data, output_csv)

    last_csv_file = output_csv
    messagebox.showinfo("¡Éxito!", f"CSV guardado como:\n{output_csv}")

def load_csv_file():
    global last_csv_file
    file_path = filedialog.askopenfilename(filetypes=[("CSV files", "*.csv")])
    if file_path:
        last_csv_file = file_path
        messagebox.showinfo("CSV cargado", f"Archivo cargado:\n{file_path}")


# Centrar ventana
def center_window(window, width=400, height=250):
    screen_width = window.winfo_screenwidth()
    screen_height = window.winfo_screenheight()
    x = (screen_width // 2) - (width // 2)
    y = (screen_height // 2) - (height // 2)
    window.geometry(f"{width}x{height}+{x}+{y}")

# GUI Tkinter
root = tk.Tk()
root.title("Conversor y Visualizador de Sesiones")
center_window(root)

label = tk.Label(root, text="Selecciona una opción:")
label.pack(pady=10)

btn_json = tk.Button(root, text="Cargar JSON y convertir a CSV", command=load_json_file)
btn_json.pack(pady=5)

btn_csv = tk.Button(root, text="Cargar CSV existente", command=load_csv_file)
btn_csv.pack(pady=5)

btn_plot = tk.Button(root, text="Graficar último CSV cargado", command=plot_last_csv)
btn_plot.pack(pady=10)

root.mainloop()
 