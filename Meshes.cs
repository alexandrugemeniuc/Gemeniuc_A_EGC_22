using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Egc;

/// <summary>
/// Fabrică de forme geometrice predefinite.
/// Returnează instanțe de Mesh gata de utilizare.
/// </summary>
public static class Meshes
{
    /// <summary>
    /// Un cub (cutie) cu latura de 1 unitate, centrat în origine.
    /// Fiecare față are o culoare diferită pentru a fi ușor de urmărit rotația.
    /// </summary>
    public static Mesh Box
    {
        get
        {
            // 24 de vârfuri (4 pentru fiecare dintre cele 6 fețe) pentru a permite culori distincte pe fețe
            float[] vertices =
            {
                // Fața Front (Roșu)
                -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,   0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,   0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,
                // Fața Back (Verde)
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,   0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,   0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
                // Fața Top (Albastru)
                -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,
                // Fața Bottom (Galben)
                -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,   0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,   0.5f, -0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  -0.5f, -0.5f,  0.5f,  1.0f, 1.0f, 0.0f,
                // Fața Right (Magenta)
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 1.0f,   0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,   0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f,   0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 1.0f,
                // Fața Left (Cyan)
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f,  -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 1.0f,  -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 1.0f,
            };

            uint[] indices =
            {
                0, 1, 2,  2, 3, 0,       // Front
                4, 5, 6,  6, 7, 4,       // Back
                8, 9, 10, 10, 11, 8,     // Top
                12, 13, 14, 14, 15, 12,  // Bottom
                16, 17, 18, 18, 19, 16,  // Right
                20, 21, 22, 22, 23, 20   // Left
            };

            return new Mesh(vertices, indices, PrimitiveType.Triangles);
        }
    }

    /// <summary>
    /// Un cilindru vertical, centrat în origine.
    /// </summary>
    public static Mesh Cylinder
    {
        get
        {
            return CreateCylinder(radius: 0.5f, height: 1.0f, segments: 32);
        }
    }

    /// <summary>
    /// Metodă helper pentru generarea unui cilindru cu număr variabil de segmente.
    /// </summary>
    private static Mesh CreateCylinder(float radius, float height, int segments)
    {
        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();

        float halfH = height / 2.0f;
        float angleStep = 2.0f * MathF.PI / segments;

        // Centrul capacului de sus (index 0)
        vertices.AddRange(new float[] { 0, halfH, 0, 0.8f, 0.2f, 0.2f });
        // Centrul capacului de jos (index 1)
        vertices.AddRange(new float[] { 0, -halfH, 0, 0.8f, 0.2f, 0.2f });

        int topCenterIdx = 0;
        int botCenterIdx = 1;
        int currentIdx = 2;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = MathF.Cos(angle) * radius;
            float z = MathF.Sin(angle) * radius;

            // Vârf pe marginea de sus
            vertices.AddRange(new float[] { x, halfH, z, 0.9f, 0.9f, 0.9f });
            // Vârf pe marginea de jos
            vertices.AddRange(new float[] { x, -halfH, z, 0.9f, 0.9f, 0.9f });

            int topIdx = currentIdx;
            int botIdx = currentIdx + 1;
            int nextTopIdx = (i == segments - 1) ? 2 : currentIdx + 2;
            int nextBotIdx = (i == segments - 1) ? 3 : currentIdx + 3;

            // 1. Triunghi pentru capacul de sus
            indices.Add((uint)topCenterIdx);
            indices.Add((uint)topIdx);
            indices.Add((uint)nextTopIdx);

            // 2. Triunghi pentru capacul de jos (ordinea inversată pentru culling corect)
            indices.Add((uint)botCenterIdx);
            indices.Add((uint)nextBotIdx);
            indices.Add((uint)botIdx);

            // 3. Fața laterală (2 triunghiuri pentru a forma un pătrat)
            indices.Add((uint)topIdx);
            indices.Add((uint)botIdx);
            indices.Add((uint)nextBotIdx);

            indices.Add((uint)topIdx);
            indices.Add((uint)nextBotIdx);
            indices.Add((uint)nextTopIdx);

            currentIdx += 2;
        }

        return new Mesh(vertices.ToArray(), indices.ToArray(), PrimitiveType.Triangles);
    }
}