import type { ResourceTypeDto } from '../types/api';
import { ResourceSprite } from './sprites';

interface ResourceNameChipProps {
    resourceType: ResourceTypeDto;
}

/**
 * Плашка ресурса именем, а не числом: подставляется в предложение там, где имя пришлось бы склонять.
 */
export const ResourceNameChip = ({ resourceType }: ResourceNameChipProps) => (
    <span className="resource-chip resource-chip-name">
        <ResourceSprite logicName={resourceType.logicName} aria-hidden="true" />
        {resourceType.name}
    </span>
);
