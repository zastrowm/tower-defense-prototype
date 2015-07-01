using System;
using System.Collections.Generic;
using System.Linq;

namespace NineByteGames.TowerDefense.Signals
{
  /// <summary> Non generic class of ISignalListener. </summary>
  public interface ISignalListener
  {
  }

  /// <summary> A child which is interested in various signals. </summary>
  /// <typeparam name="T"> The type of data that the object can handle. </typeparam>
  public interface ISignalListener<T> : ISignalListener
  {
    /// <summary> Handle the given message. </summary>
    /// <param name="message"> The message/data to handle. </param>
    /// <returns>
    ///  HandleResult.Handled if the message was consumed, HandleResult.Unhandled if it should be
    ///  handled by the next listener.
    /// </returns>
    void Handle(T message);
  }
}