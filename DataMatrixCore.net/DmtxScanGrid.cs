/*
DataMatrix.Net

DataMatrix.Net - .net library for decoding DataMatrix codes.
Copyright (C) 2009/2010 Michael Faschinger

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public
License as published by the Free Software Foundation; either
version 3.0 of the License, or (at your option) any later version.
You can also redistribute and/or modify it under the terms of the
GNU Lesser General Public License as published by the Free Software
Foundation; either version 3.0 of the License or (at your option)
any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
General Public License or the GNU Lesser General Public License 
for more details.

You should have received a copy of the GNU General Public
License and the GNU Lesser General Public License along with this 
library; if not, write to the Free Software Foundation, Inc., 
51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

Contact: Michael Faschinger - michfasch@gmx.at
 
*/

using System;

namespace DataMatrixCore.net
{
    internal struct DmtxScanGrid
    {
        #region Fields
        int _minExtent;
        int _maxExtent;
        int _xOffset;
        int _yOffset;
        int _xMin;
        int _xMax;
        int _yMin;
        int _yMax;

        int _total;
        int _extent;
        int _jumpSize;
        int _pixelTotal;
        int _startPos;

        int _pixelCount;
        int _xCenter;
        int _yCenter;
        #endregion

        #region Constructors
        internal DmtxScanGrid(DmtxDecode dec)
        {
            int smallestFeature = dec.ScanGap;
            _xMin = dec.XMin;
            _xMax = dec.XMax;
            _yMin = dec.YMin;
            _yMax = dec.YMax;

            /* Values that get set once */
            int xExtent = _xMax - _xMin;
            int yExtent = _yMax - _yMin;
            int maxExtent = xExtent > yExtent ? xExtent : yExtent;

            if (maxExtent < 1)
            {
                throw new ArgumentException("Invalid max extent for Scan Grid: Must be greater than 0");
            }

            int extent = 1;
            _minExtent = extent;
            for (; extent < maxExtent; extent = (extent + 1) * 2 - 1)
            {
                if (extent <= smallestFeature)
                {
                    _minExtent = extent;
                }
            }

            _maxExtent = extent;

            _xOffset = (_xMin + _xMax - _maxExtent) / 2;
            _yOffset = (_yMin + _yMax - _maxExtent) / 2;

            /* Values that get reset for every level */
            _total = 1;
            _extent = _maxExtent;

            _jumpSize = _extent + 1;
            _pixelTotal = 2 * _extent - 1;
            _startPos = _extent / 2;
            _pixelCount = 0;
            _xCenter = _yCenter = _startPos;

            SetDerivedFields();
        }
        #endregion


        #region Methods
        internal DmtxRange PopGridLocation(ref DmtxPixelLoc loc)
        {
            DmtxRange locStatus;

            do
            {
                locStatus = GetGridCoordinates(ref loc);

                /* Always leave grid pointing at next available location */
                _pixelCount++;

            } while (locStatus == DmtxRange.DmtxRangeBad);

            return locStatus;
        }

        private DmtxRange GetGridCoordinates(ref DmtxPixelLoc locRef)
        {

            /* Initially pixelCount may fall beyond acceptable limits. Update grid
             * state before testing coordinates */

            /* Jump to next cross pattern horizontally if current column is done */
            if (_pixelCount >= _pixelTotal)
            {
                _pixelCount = 0;
                _xCenter += _jumpSize;
            }

            /* Jump to next cross pattern vertically if current row is done */
            if (_xCenter > _maxExtent)
            {
                _xCenter = _startPos;
                _yCenter += _jumpSize;
            }

            /* Increment level when vertical step goes too far */
            if (_yCenter > _maxExtent)
            {
                _total *= 4;
                _extent /= 2;
                SetDerivedFields();
            }

            if (_extent == 0 || _extent < _minExtent)
            {
                locRef.X = locRef.Y = -1;
                return DmtxRange.DmtxRangeEnd;
            }

            int count = _pixelCount;

            if (count >= _pixelTotal)
            {
                throw new Exception("Scangrid is beyong image limits!");
            }

            DmtxPixelLoc loc = new DmtxPixelLoc();
            if (count == _pixelTotal - 1)
            {
                /* center pixel */
                loc.X = _xCenter;
                loc.Y = _yCenter;
            }
            else
            {
                int half = _pixelTotal / 2;
                int quarter = half / 2;

                /* horizontal portion */
                if (count < half)
                {
                    loc.X = _xCenter + (count < quarter ? count - quarter : half - count);
                    loc.Y = _yCenter;
                }
                /* vertical portion */
                else
                {
                    count -= half;
                    loc.X = _xCenter;
                    loc.Y = _yCenter + (count < quarter ? count - quarter : half - count);
                }
            }

            loc.X += _xOffset;
            loc.Y += _yOffset;

            locRef.X = loc.X;
            locRef.Y = loc.Y;

            if (loc.X < _xMin || loc.X > _xMax ||
                  loc.Y < _yMin || loc.Y > _yMax)
            {
                return DmtxRange.DmtxRangeBad;
            }

            return DmtxRange.DmtxRangeGood;
        }


        /// <summary>
        /// Update derived fields based on current state
        /// </summary>
        private void SetDerivedFields()
        {
            _jumpSize = _extent + 1;
            _pixelTotal = 2 * _extent - 1;
            _startPos = _extent / 2;
            _pixelCount = 0;
            _xCenter = _yCenter = _startPos;
        }
        #endregion

        #region Properties
        /// <summary>
        ///  Smallest cross size used in scan
        /// </summary>
        internal int MinExtent
        {
            get { return _minExtent; }
            set { _minExtent = value; }
        }

        /// <summary>
        /// Size of bounding grid region (2^N - 1)
        /// </summary>
        internal int MaxExtent
        {
            get { return _maxExtent; }
            set { _maxExtent = value; }
        }

        /// <summary>
        /// Offset to obtain image X coordinate
        /// </summary>
        internal int XOffset
        {
            get { return _xOffset; }
            set { _xOffset = value; }
        }

        /// <summary>
        /// Offset to obtain image Y coordinate
        /// </summary>
        internal int YOffset
        {
            get { return _yOffset; }
            set { _yOffset = value; }
        }

        /// <summary>
        ///  Minimum X in image coordinate system
        /// </summary>
        internal int XMin
        {
            get { return _xMin; }
            set { _xMin = value; }
        }


        /// <summary>
        /// Maximum X in image coordinate system
        /// </summary>
        internal int XMax
        {
            get { return _xMax; }
            set { _xMax = value; }
        }

        /// <summary>
        ///  Minimum Y in image coordinate system
        /// </summary>
        internal int YMin
        {
            get { return _yMin; }
            set { _yMin = value; }
        }

        /// <summary>
        /// Maximum Y in image coordinate system
        /// </summary>
        internal int YMax
        {
            get { return _yMax; }
            set { _yMax = value; }
        }

        /// <summary>
        ///  Total number of crosses at this size
        /// </summary>
        internal int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        /// <summary>
        ///  Length/width of cross in pixels
        /// </summary>
        internal int Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        /// <summary>
        /// Distance in pixels between cross centers
        /// </summary>
        internal int JumpSize
        {
            get { return _jumpSize; }
            set { _jumpSize = value; }
        }

        /// <summary>
        ///  Total pixel count within an individual cross path
        /// </summary>
        internal int PixelTotal
        {
            get { return _pixelTotal; }
            set { _pixelTotal = value; }
        }

        /// <summary>
        /// X and Y coordinate of first cross center in pattern
        /// </summary>
        internal int StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        /// <summary>
        /// Progress (pixel count) within current cross pattern
        /// </summary>
        internal int PixelCount
        {
            get { return _pixelCount; }
            set { _pixelCount = value; }
        }

        /// <summary>
        /// X center of current cross pattern
        /// </summary>
        internal int XCenter
        {
            get { return _xCenter; }
            set { _xCenter = value; }
        }


        /// <summary>
        /// Y center of current cross pattern
        /// </summary>
        internal int YCenter
        {
            get { return _yCenter; }
            set { _yCenter = value; }
        }
        #endregion
    }
}
