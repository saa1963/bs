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
    internal struct DmtxBresLine
    {
        #region Fields
        int _xStep;
        int _yStep;
        int _xDelta;
        int _yDelta;
        bool _steep;
        int _xOut;
        int _yOut;
        int _travel;
        int _outward;
        int _error;
        DmtxPixelLoc _loc;
        DmtxPixelLoc _loc0;
        DmtxPixelLoc _loc1;
        #endregion

        #region Constructors
        internal DmtxBresLine(DmtxBresLine orig)
        {
            _error = orig._error;
            _loc = new DmtxPixelLoc { X = orig._loc.X, Y = orig._loc.Y };
            _loc0 = new DmtxPixelLoc { X = orig._loc0.X, Y = orig._loc0.Y };
            _loc1 = new DmtxPixelLoc { X = orig._loc1.X, Y = orig._loc1.Y };
            _outward = orig._outward;
            _steep = orig._steep;
            _travel = orig._travel;
            _xDelta = orig._xDelta;
            _xOut = orig._xOut;
            _xStep = orig._xStep;
            _yDelta = orig._yDelta;
            _yOut = orig._yOut;
            _yStep = orig._yStep;
        }

        internal DmtxBresLine(DmtxPixelLoc loc0, DmtxPixelLoc loc1, DmtxPixelLoc locInside)
        {
            int cp;
            DmtxPixelLoc locBeg, locEnd;


            /* Values that stay the same after initialization */
            _loc0 = loc0;
            _loc1 = loc1;
            _xStep = loc0.X < loc1.X ? +1 : -1;
            _yStep = loc0.Y < loc1.Y ? +1 : -1;
            _xDelta = Math.Abs(loc1.X - loc0.X);
            _yDelta = Math.Abs(loc1.Y - loc0.Y);
            _steep = _yDelta > _xDelta;

            /* Take cross product to determine outward step */
            if (_steep)
            {
                /* Point first vector up to get correct sign */
                if (loc0.Y < loc1.Y)
                {
                    locBeg = loc0;
                    locEnd = loc1;
                }
                else
                {
                    locBeg = loc1;
                    locEnd = loc0;
                }
                cp = (locEnd.X - locBeg.X) * (locInside.Y - locEnd.Y) -
                      (locEnd.Y - locBeg.Y) * (locInside.X - locEnd.X);

                _xOut = cp > 0 ? +1 : -1;
                _yOut = 0;
            }
            else
            {
                /* Point first vector left to get correct sign */
                if (loc0.X > loc1.X)
                {
                    locBeg = loc0;
                    locEnd = loc1;
                }
                else
                {
                    locBeg = loc1;
                    locEnd = loc0;
                }
                cp = (locEnd.X - locBeg.X) * (locInside.Y - locEnd.Y) -
                      (locEnd.Y - locBeg.Y) * (locInside.X - locEnd.X);

                _xOut = 0;
                _yOut = cp > 0 ? +1 : -1;
            }

            /* Values that change while stepping through line */
            _loc = loc0;
            _travel = 0;
            _outward = 0;
            _error = _steep ? _yDelta / 2 : _xDelta / 2;
        }
        #endregion

        #region Methods
        internal bool GetStep(DmtxPixelLoc target, ref int travel, ref int outward)
        {
            /* Determine necessary step along and outward from Bresenham line */
            if (_steep)
            {
                travel = _yStep > 0 ? target.Y - _loc.Y : _loc.Y - target.Y;
                Step(travel, 0);
                outward = _xOut > 0 ? target.X - _loc.X : _loc.X - target.X;
                if (_yOut != 0)
                {
                    throw new Exception("Invald yOut value for bresline step!");
                }
            }
            else
            {
                travel = _xStep > 0 ? target.X - _loc.X : _loc.X - target.X;
                Step(travel, 0);
                outward = _yOut > 0 ? target.Y - _loc.Y : _loc.Y - target.Y;
                if (_xOut != 0)
                {
                    throw new Exception("Invald xOut value for bresline step!");
                }
            }

            return true;
        }


        internal bool Step(int travel, int outward)
        {
            int i;

            if (Math.Abs(travel) >= 2)
            {
                throw new ArgumentException("Invalid value for 'travel' in BaseLineStep!");
            }

            /* Perform forward step */
            if (travel > 0)
            {
                _travel++;
                if (_steep)
                {
                    _loc = new DmtxPixelLoc() { X = _loc.X, Y = _loc.Y + _yStep };
                    _error -= _xDelta;
                    if (_error < 0)
                    {
                        _loc = new DmtxPixelLoc() { X = _loc.X + _xStep, Y = _loc.Y };
                        _error += _yDelta;
                    }
                }
                else
                {
                    _loc = new DmtxPixelLoc() { X = _loc.X + _xStep, Y = _loc.Y };
                    _error -= _yDelta;
                    if (_error < 0)
                    {
                        _loc = new DmtxPixelLoc() { X = _loc.X, Y = _loc.Y + _yStep };
                        _error += _xDelta;
                    }
                }
            }
            else if (travel < 0)
            {
                _travel--;
                if (_steep)
                {
                    _loc = new DmtxPixelLoc() { X = _loc.X, Y = _loc.Y - _yStep };
                    _error += _xDelta;
                    if (Error >= YDelta)
                    {
                        _loc = new DmtxPixelLoc() { X = _loc.X - _xStep, Y = _loc.Y };
                        _error -= _yDelta;
                    }
                }
                else
                {
                    _loc = new DmtxPixelLoc() { X = _loc.X - _xStep, Y = _loc.Y };
                    _error += _yDelta;
                    if (_error >= _xDelta)
                    {
                        _loc = new DmtxPixelLoc() { X = _loc.X, Y = _loc.Y - _yStep };
                        _error -= _xDelta;
                    }
                }
            }

            for (i = 0; i < outward; i++)
            {
                /* Outward steps */
                _outward++;
                _loc = new DmtxPixelLoc() { X = _loc.X + _xOut, Y = _loc.Y + _yOut };
            }

            return true;
        }
        #endregion

        #region Properties
        internal int XStep
        {
            get { return _xStep; }
            set { _xStep = value; }
        }

        internal int YStep
        {
            get { return _yStep; }
            set { _yStep = value; }
        }

        internal int XDelta
        {
            get { return _xDelta; }
            set { _xDelta = value; }
        }

        internal int YDelta
        {
            get { return _yDelta; }
            set { _yDelta = value; }
        }

        internal bool Steep
        {
            get { return _steep; }
            set { _steep = value; }
        }

        internal int XOut
        {
            get { return _xOut; }
            set { _xOut = value; }
        }

        internal int YOut
        {
            get { return _yOut; }
            set { _yOut = value; }
        }

        internal int Travel
        {
            get { return _travel; }
            set { _travel = value; }
        }

        internal int Outward
        {
            get { return _outward; }
            set { _outward = value; }
        }

        internal int Error
        {
            get { return _error; }
            set { _error = value; }
        }

        internal DmtxPixelLoc Loc
        {
            get { return _loc; }
            set { _loc = value; }
        }


        internal DmtxPixelLoc Loc0
        {
            get { return _loc0; }
            set { _loc0 = value; }
        }

        internal DmtxPixelLoc Loc1
        {
            get { return _loc1; }
            set { _loc1 = value; }
        }
        #endregion
    }
}
