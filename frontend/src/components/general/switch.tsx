import React from 'react';

type ToggleSwitchProps = {
  isOn: boolean;
  onToggle: () => void;
  label?: string;
};

const ToggleSwitch: React.FC<ToggleSwitchProps> = ({ isOn, onToggle, label }) => {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
      {label && <span>{label}</span>}
      <div
        onClick={onToggle}
        style={{
          width: '40px',
          height: '20px',
          borderRadius: '20px',
          background: isOn ? 'green' : 'gray',
          display: 'flex',
          alignItems: 'center',
          justifyContent: isOn ? 'flex-end' : 'flex-start',
          padding: '2px',
          cursor: 'pointer',
          transition: 'background 0.3s, justify-content 0.3s',
        }}
      >
        <div
          style={{
            width: '16px',
            height: '16px',
            borderRadius: '50%',
            background: 'white',
            boxShadow: '0 0 2px rgba(0, 0, 0, 0.3)',
          }}
        />
      </div>
    </div>
  );
};

export default ToggleSwitch;
