using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace PNNLControls {
  /// <summary>
  /// This ‘NumEditBox’ is more powerful than other implementations that test the key input 
  /// stroke by stroke. It allows not only pasting a full value, but also blind typing 
  /// and correcting in the end. The component may accept integer or float values by choice 
  /// and can recognize various formats. The original event was not consumed internally; 
  /// it can still be used ‘outside the box’ to extend the functionality in various ways.
  /// </summary>
  [DesignerCategory("Code")]
  public class NumEditBox : TextBox {

    public NumEditBox():base() {
      base.Validating += new CancelEventHandler(this.numValidating);
    }

    #region new properties
    private float _Value;
    [Category("Appearance"),
    DefaultValue(0),
    Description("Convert it to integer when necesary.")]
    public float Value {
      get { return _Value; }
      set { 
        _Value = value; 
/* not a good idea
        if (ToValidate==ValidateType.Integer) 
          Text = System.Convert.ToInt32(_Value).ToString();
        else
          Text = _Value.ToString();
*/
      }
    }

    private ValidateType _ToValidate;
    [Category("Appearance"),
    DefaultValue(ValidateType.Integer)]
    public ValidateType ToValidate {
      get { return _ToValidate; }
      set { _ToValidate = value; }
    }

    private NumberStyles _NumStyles= NumberStyles.Number | NumberStyles.AllowExponent;
    [Category("Appearance"),
    DefaultValue(NumberStyles.Number | NumberStyles.AllowExponent),
    Description("Customize the numeric validation number style.")]
    public NumberStyles NumStyles {
      get { return _NumStyles; }
      set { _NumStyles = value; }
    }
    #endregion

    #region events
    public new event CancelEventHandler Validating;

    private void numValidating(object sender, System.ComponentModel.CancelEventArgs e) {
       if (Validating!=null) {
        Validating(sender,e);
        if (!e.Cancel) return;
        e.Cancel=false;
      }
      string s = Text;
      int l = s.Length;
      while (s.Length>0) {
        try {
          _Value = (ToValidate==ValidateType.Integer) ? int.Parse(s, NumStyles) : float.Parse(s, NumStyles);
          break;
        } catch {
          s = s.Substring(0,s.Length-1);
        }
        if (s.Length!=l) {
          SelectionStart=s.Length;
          SelectionLength=l-s.Length;
          e.Cancel=true;
        }
      }
    }

    #endregion
  } // class NumEditBox

  public enum ValidateType {
    Integer,
    Float
  }

}
