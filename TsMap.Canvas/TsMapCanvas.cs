﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace TsMap.Canvas
{
    public partial class TsMapCanvas : Form
    {
        private readonly TsMapRenderer _renderer;

        private PointF _pos;
        private Point? _dragPoint;

        private float _mapScale = 4000;

        private ImageExportOptionForm _imageExportForm;

        private RenderFlags _renderFlags = RenderFlags.All;

        private MapPalette _palette;

        public TsMapCanvas(Form f, string path, List<Mod> mods)
        {
            InitializeComponent();

            var mapper = new TsMapper(path, mods);
            _palette = new SimpleMapPalette();

            if (path.Contains("American Truck Simulator"))
            {
                _pos = new PointF(-103000, -54444);
                mapper.IsEts2 = false;
            }
            else
            {
                _pos = new PointF(850, -920);
                mapper.IsEts2 = true;
            }

            mapper.Parse();

            _renderer = new TsMapRenderer(mapper);

            Timer t = new Timer
            {
                Interval = 1000 / 30
            };
            t.Tick += (s, a) => MapPanel.Invalidate();
            t.Start();

            // Panning around
            MapPanel.MouseDown += (s, e) => _dragPoint = e.Location;
            MapPanel.MouseUp += (s, e) => _dragPoint = null;
            MapPanel.MouseMove += (s, e) =>
            {
                if (_dragPoint == null) return;
                var spd = _mapScale / Math.Max(this.Width, this.Height);
                _pos.X = _pos.X - (e.X - _dragPoint.Value.X) * spd;
                _pos.Y = _pos.Y - (e.Y - _dragPoint.Value.Y) * spd;
                _dragPoint = e.Location;
            };

            MapPanel.MouseWheel += TsMapCanvas_MouseWheel;

            MapPanel.Resize += TsMapCanvas_Resize;

            Closed += (s, e) =>
            {
                f.Close();
                _imageExportForm?.Close();
            };

        }

        private void TsMapCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            _mapScale -= e.Delta;
            _mapScale = Math.Max(100, Math.Min(60000, _mapScale));
        }

        private void TsMapCanvas_Resize(object sender, System.EventArgs e)
        {
            MapPanel.Invalidate();
        }

        private void ExportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_imageExportForm == null || _imageExportForm.IsDisposed) _imageExportForm = new ImageExportOptionForm();
            _imageExportForm.Show();
            _imageExportForm.BringToFront();

            _imageExportForm.ExportImage += (width, height) => // Called when export button is pressed in ImageExportOptionForm
            {
                if (width == 0 || height == 0) return;
                var bitmap = new Bitmap(width, height);

                _renderer.Render(Graphics.FromImage(bitmap), new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    _mapScale, _pos, _palette, _renderFlags);

                var result = exportFileDialog.ShowDialog();
                if (result != DialogResult.OK) return;

                var fileStream = exportFileDialog.OpenFile();

                bitmap.Save(fileStream, ImageFormat.Png);
                fileStream.Close();
                _imageExportForm.Hide();
            };
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ItemVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var itemVisibilityForm = new ItemVisibilityForm(_renderFlags);
            itemVisibilityForm.Show();
            itemVisibilityForm.BringToFront();

            itemVisibilityForm.UpdateItemVisibility += (renderFlags) =>
            {
                _renderFlags = renderFlags;
            };
        }

        private void paletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paletteEditorForm = new PaletteEditorForm(_palette);
            paletteEditorForm.Show();
            paletteEditorForm.BringToFront();

            paletteEditorForm.UpdatePalette += (palette) =>
            {
                _palette = palette;
            };
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            _renderer.Render(e.Graphics, e.ClipRectangle, _mapScale, _pos, _palette, _renderFlags);
        }
    }
}
