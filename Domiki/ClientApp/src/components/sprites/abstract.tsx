import ReputationSprite from '../../assets/abstract/reputation.svg?react';
import WorkerSkillSprite from '../../assets/abstract/worker_skill.svg?react';
import FatigueRestSprite from '../../assets/abstract/fatigue_rest.svg?react';
import PrestigeSprite from '../../assets/abstract/prestige_new_valley.svg?react';
import ProductionRecipeSprite from '../../assets/abstract/production_recipe.svg?react';
import NearSortieSprite from '../../assets/abstract/near_sortie.svg?react';
import LongExpeditionSprite from '../../assets/abstract/long_expedition.svg?react';
import WalkingSortieSprite from '../../assets/abstract/walking_sortie.svg?react';
import RareExpeditionFindSprite from '../../assets/abstract/rare_expedition_find.svg?react';
import ExpeditionHardeningSprite from '../../assets/abstract/expedition_hardening.svg?react';
import BlueprintSprite from '../../assets/abstract/blueprint.svg?react';
import UntouchedDepositsSprite from '../../assets/abstract/untouched_deposits.svg?react';
import JournalAbstractSprite from '../../assets/abstract/journal.svg?react';
import ElderOrderSprite from '../../assets/abstract/elder_order.svg?react';
import HouseholdAbstractSprite from '../../assets/abstract/household.svg?react';
import SmartArtelSprite from '../../assets/abstract/smart_artel.svg?react';
import HurrySprite from '../../assets/abstract/hurry.svg?react';
import IncidentSprite from '../../assets/abstract/incident.svg?react';
import WorkerMilestoneSprite from '../../assets/abstract/worker_milestone.svg?react';
import GoalSprite from '../../assets/abstract/goal.svg?react';
import WorkerMasterySprite from '../../assets/abstract/worker_mastery.svg?react';
import ExpeditionGenericSprite from '../../assets/abstract/expedition_generic.svg?react';
import { renderIconSprite } from './core';
import type { IconSpriteProps, SpriteComponent } from './core';

const abstractSprites: Record<string, SpriteComponent> = {
    reputation: ReputationSprite,
    worker_skill: WorkerSkillSprite,
    fatigue_rest: FatigueRestSprite,
    prestige_new_valley: PrestigeSprite,
    production_recipe: ProductionRecipeSprite,
    near_sortie: NearSortieSprite,
    long_expedition: LongExpeditionSprite,
    walking_sortie: WalkingSortieSprite,
    rare_expedition_find: RareExpeditionFindSprite,
    expedition_hardening: ExpeditionHardeningSprite,
    blueprint: BlueprintSprite,
    untouched_deposits: UntouchedDepositsSprite,
    journal: JournalAbstractSprite,
    elder_order: ElderOrderSprite,
    household: HouseholdAbstractSprite,
    smart_artel: SmartArtelSprite,
    hurry: HurrySprite,
    incident: IncidentSprite,
    worker_milestone: WorkerMilestoneSprite,
    goal: GoalSprite,
    worker_mastery: WorkerMasterySprite,
    expedition_generic: ExpeditionGenericSprite,
};

export const AbstractSprite = (props: IconSpriteProps) => <>{renderIconSprite('abstract', abstractSprites, undefined, props)}</>;
