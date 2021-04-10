﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace chess
{
    public static class Board
    {
        private static readonly int _tileWidth = 128;
        private static readonly int _tileHeight = 128;

        private static Color[] _colorDark = new Color[] { Color.Black };
        private static Color[] _colorLight = new Color[] { Color.White };
        private static Color[] _colorSelectedTile = new Color[_tileWidth * _tileHeight];
        private static Color[] _colorPossibleMove = new Color[_tileWidth * _tileHeight];

        private static Texture2D _textureDark;
        private static Texture2D _textureLight;
        private static Texture2D _textureSelectedTile;
        private static Texture2D _texturePossibleMove;

        private static int _boardOffsetHoriz = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 3.5f);
        private static int _boardOffsetVert = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 6;

        private static Tile[,] _tiles = new Tile[8, 8];
        private static Tile _selectedTile;

        /// <summary>
        /// Creates all tiles needed for a full board.
        /// </summary>
        private static void CreateTiles()
        {
            Texture2D firstColor = _textureLight;
            Texture2D curColor = firstColor;
            for (int i = 0; i < 8; i++)
            {
                int y = _boardOffsetVert + i * _tileHeight;
                for (int j = 0; j < 8; j++)
                {
                    int x = _boardOffsetHoriz + j * _tileWidth;
                    // This adds the tile with matrix indexing, i.e (row, col), NOT (X, Y).
                    _tiles[i, j] = new Tile(curColor, _tileWidth, _tileHeight, (i, j), new Rectangle(x, y, _tileWidth, _tileHeight));


                    if (j != 7) curColor = SwitchCurrentColor(curColor); //Don't switch color last tile
                }
            }
        }

        private static Texture2D SwitchCurrentColor(Texture2D currentColor)
        {
            if (currentColor == _textureLight)
            {
                return _textureDark;
            }
            return _textureLight;
        }

        /// <summary>
        /// Sets up the board. Must be called first, before any other call.
        /// </summary>
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            // These for-loops sets the color of each pixel
            for (int i = 0; i < _colorSelectedTile.Length; i++)
            {
                _colorSelectedTile[i] = new Color(94, 191, 21);
            }

            for (int i = 0; i < _colorPossibleMove.Length; i++)
            {
                _colorPossibleMove[i] = new Color(50, 75, 119);
            }

            _textureDark = new Texture2D(graphicsDevice, 1, 1);
            _textureDark.SetData(_colorDark);

            _textureLight = new Texture2D(graphicsDevice, 1, 1);
            _textureLight.SetData(_colorLight);

            _texturePossibleMove = new Texture2D(graphicsDevice, _tileWidth, _tileHeight);
            _texturePossibleMove.SetData(_colorPossibleMove);
            _texturePossibleMove.CreateBorder(Color.DarkOrange, 2, true);

            _textureSelectedTile = new Texture2D(graphicsDevice, _tileWidth, _tileHeight);
            _textureSelectedTile.SetData(_colorSelectedTile);
            _textureSelectedTile.CreateBorder(Color.Beige, 2, false);

            CreateTiles();
        }

        /// <summary>
        /// Update the board.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draw all board tiles.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile t in _tiles)
            {
                t.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Highlights, or removes the highlights, of the tiles a piece can move to. 
        /// </summary>
        /// <param name="moves">The possible moves of the selected piece. (Tile coordinates)</param>
        /// <param name="removeHighlight">Removes highlights if set to true.</param>
        public static void HighlightPossibleMoves(List<(int, int)> moves, bool removeHighlight)
        {
            foreach (var move in moves)
            {
                Tile t = GetTileAtIndex(move);
                // If the tile is out of bounds, skip to the next move
                if (t == null) continue;

                if (!removeHighlight) t.IsPossibleMove = true;
                else                  t.IsPossibleMove = false;
            }
        }

        /// <summary>
        /// Returns the tile at the given coordinate. Returns null if the coordinates are out of bounds.
        /// </summary>
        /// <param name="cor">Tile coordinate.</param>
        /// <returns>The tile at the specified coordinate, or null if out of bounds.</returns>
        public static Tile GetTileAtIndex((int, int) cor)
        {
            if (IsTileInBounds(cor))
                return _tiles[cor.Item1, cor.Item2];
            else
                return null;
        }

        /// <summary>
        /// Check if the specified coordinate is in the bounds of the chess board.
        /// </summary>
        /// <param name="cor">The coordinate to check.</param>
        /// <returns>true if in bounds; false if out of bounds.</returns>
        public static bool IsTileInBounds((int, int) cor)
        {
            return cor.Item1 >= 0 && cor.Item1 < 8 && cor.Item2 >= 0 && cor.Item2 < 8;
        }


        /// <returns>The color for the dark tiles.</returns>
        public static Color[] ColorDark
        {
            get => _colorDark;
        }

        /// <returns>The color for the light tiles.</returns>
        public static Color[] ColorLight
        {
            get => _colorLight;
        }

        /// <returns>A 2D-array containing all tiles on the board.</returns>
        public static Tile[,] Tiles
        {
            get => _tiles;
        }

        public static Tile SelectedTile
        {
            get => _selectedTile;
            set => _selectedTile = value;
        }

        public static Texture2D TextureSelectedTile
        {
            get => _textureSelectedTile;
        }

        public static Texture2D TexturePossibleMove
        {
            get => _texturePossibleMove;
        }
    }

    /// <summary>
    /// Tile for a chess board.
    /// </summary>
    public class Tile
    {
        private Texture2D _texture;

        private Rectangle _pos;
        private (int, int) _coordinate;
        private int _width;
        private int _height;

        private bool _isPossibleMove;

        /// <summary>
        /// Creates a new tile.
        /// </summary>
        /// <param name="texture">The texture of this tile.</param>
        /// <param name="width">Tile width.</param>
        /// <param name="height">Tile height.</param>
        /// <param name="coordinate">Tile coordinate.</param>
        /// <param name="pos">Tile position on screen.</param>
        public Tile(Texture2D texture, int width, int height, (int, int) coordinate, Rectangle pos)
        {
            _texture = texture;
            _width = width;
            _height = height;
            _coordinate = coordinate;
            _pos = pos;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _pos, Color.White);

            if (Board.SelectedTile == this)
                spriteBatch.Draw(Board.TextureSelectedTile, _pos, Color.White);             

            if (_isPossibleMove)
                spriteBatch.Draw(Board.TexturePossibleMove, _pos, Color.White);
        }

        /// <returns>This tile's position on the screen.</returns>
        public Rectangle Position
        {
            get => _pos;
        }

        /// <returns>This tile's coordinate.</returns>
        public (int, int) Coordinate
        {
            get => _coordinate;
        }

        public bool IsPossibleMove
        {
            get => _isPossibleMove;
            set => _isPossibleMove = value;
        }
    }
}
