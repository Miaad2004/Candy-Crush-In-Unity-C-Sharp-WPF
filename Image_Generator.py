import sqlite3
import os
import random

# Directory containing the images
image_directory = r"D:\CS\Datasets\celeb\img_align_celeba\img_align_celeba"

# SQLite database file path
database_file = "C:\\Users\\Miaad\\Desktoptssehsyst.db"

# Connect to the database
conn = sqlite3.connect(database_file)
cursor = conn.cursor()

# Get a list of all image files in the directory
image_files = [os.path.join(image_directory, f) for f in os.listdir(image_directory) if os.path.isfile(os.path.join(image_directory, f))]

# Retrieve all player records from the Players table
cursor.execute("SELECT Id FROM Players")
player_ids = cursor.fetchall()

# Update the ProfileImagePath randomly for each player
for player_id in player_ids:
    profile_image_path = random.choice(image_files)
    cursor.execute("UPDATE Players SET ProfileImagePath=? WHERE Id=?", (profile_image_path, player_id[0]))

# Commit the changes and close the connection
conn.commit()
conn.close()

print("ProfileImagePath updated successfully!")
