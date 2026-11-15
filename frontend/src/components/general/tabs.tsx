
import React, { useState, ReactNode } from 'react';

type Tab = {
  label: string;
  content: ReactNode;
};

type TabsProps = {
  tabs: Tab[];
};

const Tabs: React.FC<TabsProps> = ({ tabs }) => {
  const [activeTabIndex, setActiveTabIndex] = useState(0);

  return (
    <div>
      <div style={{ display: 'flex', cursor: 'pointer' }}>
        {tabs.map((tab, index) => (
          <div
            key={index}
            onClick={() => setActiveTabIndex(index)}
            style={{
              padding: '10px 20px',
              fontWeight: activeTabIndex === index ? 'bold' : 'normal',
            }}
          >
            {tab.label}
          </div>
        ))}
      </div>

      {/* Tab Content */}
      <div style={{ padding: '20px' }}>
        {tabs[activeTabIndex].content}
      </div>
    </div>
  );
};

export default Tabs;